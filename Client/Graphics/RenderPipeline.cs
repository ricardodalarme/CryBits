using Arch.System;
using CryBits.Client.Framework;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Logic;
using CryBits.Client.Managers;
using CryBits.Client.Network;
using CryBits.Client.Systems.Character;
using CryBits.Client.Systems.Core;
using CryBits.Client.Systems.Map;
using CryBits.Client.Worlds;
using CryBits.Enums;
using Color = SFML.Graphics.Color;

namespace CryBits.Client.Graphics;

internal sealed class RenderPipeline(
    Renderer renderer,
    GameContext context,
    CameraManager cameraManager,
    MapRenderer mapRenderer,
    UIRenderer uiRenderer)
{
    public static RenderPipeline Instance { get; } = new(
        Renderer.Instance,
        GameContext.Instance,
        CameraManager.Instance,
        MapRenderer.Instance,
        UIRenderer.Instance);

    // Ground-layer render systems: non-character sprites (ground items, blood splats).
    private readonly Group<int> _groundRenderSystems = new(
        "GroundRenderSystems",
        new SpriteRenderSystem(context.World, renderer)
    );

    // Character render systems: Y-sorted shadow + animated sprite for all characters.
    // Runs after ground items, before the fringe tile layer, so characters appear on top
    // of the ground but behind foreground props.
    private readonly Group<int> _characterRenderSystems = new(
        "CharacterRenderSystems",
        new CharacterRenderSystem(context.World, renderer)
    );

    // Fringe-layer render systems: scrolling fog drawn after the
    // foreground tile pass so they sit above all world geometry.
    private readonly Group<int> _fringeRenderSystems = new(
        "FringeRenderSystems",
        new ScrollingOverlayRenderSystem(context.World, renderer)
    );

    // Weather render systems: particle batch + lightning overlay, drawn after fringe tiles.
    private readonly Group<int> _weatherRenderSystems = new(
        "WeatherRenderSystems",
        new WeatherRenderSystem(context.World, context, renderer)
    );

    // HUD-layer render systems: vital bars drawn above names but below fixed-position UI.
    private readonly Group<int> _hudRenderSystems = new(
        "HudRenderSystems",
        new VitalBarRenderSystem(context.World, renderer)
    );

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

        // Update camera logic and apply the SFML view.
        // All subsequent draws happen in world-space coordinates.
        cameraManager.Update();
        cameraManager.BeginWorldDraw();

        // Ground layer — panorama, tiles, then non-character world objects.
        mapRenderer.DrawPanorama();
        mapRenderer.DrawLayer((byte)Layer.Ground);
        _groundRenderSystems.Update(0);

        // Character layer — Y-sorted shadow + animated sprite for players and NPCs.
        _characterRenderSystems.Update(0);

        // Foreground tile layer and atmospheric effects.
        mapRenderer.DrawLayer((byte)Layer.Fringe);
        _weatherRenderSystems.Update(0);

        // Fringe systems — floating names, fog overlay.
        _fringeRenderSystems.Update(0);

        // HUD layer — vital bars drawn above names.
        _hudRenderSystems.Update(0);

        mapRenderer.DrawMapName();
        uiRenderer.DrawParty();

        // FPS/Latency overlays.
        if (Options.Instance.ShowMetrics) renderer.DrawText("FPS: " + GameLoop.Fps, 176, 7, Color.White);
        if (Options.Instance.ShowMetrics) renderer.DrawText("Latency: " + NetworkClient.Latency, 176, 19, Color.White);
    }
}
