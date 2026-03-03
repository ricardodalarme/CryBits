using Arch.System;
using CryBits.Client.Entities;
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

internal sealed class RenderPipeline
{
    public static RenderPipeline Instance { get; } = new();

    private readonly Renderer _renderer = Renderer.Instance;
    private readonly GameContext _context = GameContext.Instance;
    private readonly CameraManager _cameraManager = CameraManager.Instance;
    private readonly MapRenderer _mapRenderer = MapRenderer.Instance;
    private readonly UIRenderer _uiRenderer = UIRenderer.Instance;

    // Ground-layer render systems: non-character sprites (ground items, blood splats).
    private static readonly Group<int> _groundRenderSystems = new(
        "GroundRenderSystems",
        new SpriteRenderSystem(GameContext.Instance.World)
    );

    // Character render systems: Y-sorted shadow + animated sprite for all characters.
    // Runs after ground items, before the fringe tile layer, so characters appear on top
    // of the ground but behind foreground props.
    private static readonly Group<int> _characterRenderSystems = new(
        "CharacterRenderSystems",
        new CharacterRenderSystem(GameContext.Instance.World)
    );

    // Fringe-layer render systems: floating names and scrolling fog drawn after the
    // foreground tile pass so they sit above all world geometry.
    private static readonly Group<int> _fringeRenderSystems = new(
        "FringeRenderSystems",
        new TextRenderSystem(GameContext.Instance.World),
        new ScrollingOverlayRenderSystem(GameContext.Instance.World)
    );

    // Weather render systems: particle batch + lightning overlay, drawn after fringe tiles.
    private static readonly Group<int> _weatherRenderSystems = new(
        "WeatherRenderSystems",
        new WeatherRenderSystem(GameContext.Instance.World)
    );

    // HUD-layer render systems: vital bars drawn above names but below fixed-position UI.
    private static readonly Group<int> _hudRenderSystems = new(
        "HudRenderSystems",
        new VitalBarRenderSystem(GameContext.Instance.World)
    );

    /// <summary>
    /// Render the current frame: clear, draw game world and UI, then present.
    /// </summary>
    public void Present()
    {
        _renderer.RenderWindow.Clear(Color.Black);

        InGame();

        // Restore the default view before drawing UI so it renders at fixed screen positions.
        _cameraManager.BeginUIDraw();

        _uiRenderer.DrawInterface(Screen.Current?.Body);

        if (Screen.Current == Screens.Game) _uiRenderer.DrawChat();

        _renderer.RenderWindow.Display();
    }

    private void InGame()
    {
        if (Screen.Current != Screens.Game) return;

        // Update camera logic and apply the SFML view.
        // All subsequent draws happen in world-space coordinates.
        _cameraManager.Update();
        _cameraManager.BeginWorldDraw();

        // Ground layer — panorama, tiles, then non-character world objects.
        _mapRenderer.DrawPanorama();
        _mapRenderer.DrawLayer((byte)Layer.Ground);
        _groundRenderSystems.Update(0);

        // Character layer — Y-sorted shadow + animated sprite for players and NPCs.
        _characterRenderSystems.Update(0);

        // Foreground tile layer and atmospheric effects.
        _mapRenderer.DrawLayer((byte)Layer.Fringe);
        _weatherRenderSystems.Update(0);

        // Fringe systems — floating names, fog overlay.
        _fringeRenderSystems.Update(0);

        // HUD layer — vital bars drawn above names.
        _hudRenderSystems.Update(0);

        _mapRenderer.DrawMapName();
        _uiRenderer.DrawParty();

        // FPS/Latency overlays.
        if (Options.ShowMetrics) _renderer.DrawText("FPS: " + GameLoop.Fps, 176, 7, Color.White);
        if (Options.ShowMetrics) _renderer.DrawText("Latency: " + NetworkClient.Latency, 176, 19, Color.White);
    }
}
