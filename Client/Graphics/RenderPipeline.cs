using CryBits.Client.Framework;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Framework.Network;
using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Logic;
using CryBits.Client.Managers;
using CryBits.Client.Systems;
using CryBits.Enums;
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

    /// <summary>
    /// Render the current frame: clear, draw game world and UI, then present.
    /// </summary>
    public void Present()
    {
        renderer.RenderWindow.Clear(Color.Black);

        InGame();

        // Restore the default view before drawing UI so it renders at fixed screen positions.
        cameraManager.BeginUIDraw();

        uiRenderer.DrawInterface(Screen.Current?.Body);

        if (Screen.Current == Screens.Game) uiRenderer.DrawChat();

        renderer.RenderWindow.Display();
    }

    private void InGame()
    {
        if (Screen.Current != Screens.Game) return;

        // Apply the SFML view — all subsequent draws use world-space coordinates.
        cameraManager.BeginWorldDraw();

        // Ground layer — panorama, tiles, then non-character world objects.
        mapRenderer.DrawPanorama();
        mapRenderer.DrawLayer((byte)Layer.Ground);
        scheduler.GroundRender.Update(0);

        // Fringe tile layer,
        mapRenderer.DrawLayer((byte)Layer.Fringe);

        // Fringe systems
        scheduler.FringeRender.Update(0);

        mapRenderer.DrawMapName();
        uiRenderer.DrawParty();

        // FPS/Latency overlays.
        if (Options.Instance.ShowMetrics) renderer.DrawText("FPS: " + GameLoop.Fps, 176, 7, Color.White);
        if (Options.Instance.ShowMetrics) renderer.DrawText("Latency: " + NetworkClient.Latency, 176, 19, Color.White);
    }
}
