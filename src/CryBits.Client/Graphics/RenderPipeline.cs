using CryBits.Client.Framework;
using CryBits.Client.Framework.Network;
using CryBits.Client.UI;
using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Iguina;
using CryBits.Client.Managers;
using CryBits.Client.Systems;
using CryBits.Definitions.Maps;
using Color = SFML.Graphics.Color;

namespace CryBits.Client.Graphics;

internal sealed class RenderPipeline(
    Renderer renderer,
    CameraManager cameraManager,
    MapRenderer mapRenderer,
    SystemScheduler scheduler)
{
    public static RenderPipeline Instance { get; } = new(
        Renderer.Instance,
        CameraManager.Instance,
        MapRenderer.Instance,
        SystemScheduler.Instance);

    public IguinaUiRoot? IguinaUi { get; set; }

    public void Present()
    {
        renderer.RenderWindow.Clear(Color.Black);

        InGame();
        cameraManager.BeginUIDraw();
        IguinaUi?.Draw();
        renderer.RenderWindow.Display();
    }

    private void InGame()
    {
        if (GameState.CurrentScreen != ScreenType.Game) return;

        cameraManager.BeginWorldDraw();

        mapRenderer.DrawPanorama();
        mapRenderer.DrawLayer((byte)Layer.Ground);
        scheduler.GroundRender.Update(0);
        mapRenderer.DrawLayer((byte)Layer.Fringe);
        scheduler.FringeRender.Update(0);

        mapRenderer.DrawMapName();

        if (Options.Instance.ShowMetrics) renderer.DrawText("FPS: " + Game.Fps, 176, 7, Color.White);
        if (Options.Instance.ShowMetrics) renderer.DrawText("Latency: " + Connection.Latency, 176, 19, Color.White);
    }
}
