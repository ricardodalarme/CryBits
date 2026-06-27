using CryBits.Client.Framework;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Framework.Network;
using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Managers;
using CryBits.Client.Systems;
using CryBits.Definitions.Maps;
using Color = SFML.Graphics.Color;

namespace CryBits.Client.Graphics;

internal sealed class RenderPipeline(
    Renderer renderer,
    CameraManager cameraManager,
    MapRenderer mapRenderer,
    UIRenderer uiRenderer,
    SystemScheduler scheduler)
{
    public static RenderPipeline Instance { get; } = new(
        Renderer.Instance,
        CameraManager.Instance,
        MapRenderer.Instance,
        UIRenderer.Instance,
        SystemScheduler.Instance);

    public void Present()
    {
        renderer.RenderWindow.Clear(Color.Black);

        InGame();

        cameraManager.BeginUIDraw();
        DrawUI();

        renderer.RenderWindow.Display();
    }

    private void InGame()
    {
        if (Screen.Current != Screens.Game) return;

        cameraManager.BeginWorldDraw();

        mapRenderer.DrawPanorama();
        mapRenderer.DrawLayer(Layer.Ground);
        scheduler.Ground.Render();

        mapRenderer.DrawLayer(Layer.Fringe);

        scheduler.Fringe.Render();
    }

    private void DrawUI()
    {
        if (Screen.Current?.Body is { } body) uiRenderer.DrawInterface(body);

        if (Screen.Current != Screens.Game) return;

        uiRenderer.DrawChat();
        mapRenderer.DrawMapName();
        uiRenderer.DrawParty();

        if (Options.Instance.ShowMetrics) renderer.DrawText("FPS: " + Game.Fps, 176, 7, Color.White);
        if (Options.Instance.ShowMetrics) renderer.DrawText("Latency: " + Connection.Latency, 176, 19, Color.White);
    }
}
