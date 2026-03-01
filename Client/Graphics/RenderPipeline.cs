using Arch.System;
using CryBits.Client.Entities;
using CryBits.Client.Framework;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Logic;
using CryBits.Client.Network;
using CryBits.Client.Systems;
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
    private readonly PlayerRenderer _playerRenderer = PlayerRenderer.Instance;
    private readonly NpcRenderer _npcRenderer = NpcRenderer.Instance;
    private readonly UIRenderer _uiRenderer = UIRenderer.Instance;

    // Ground-layer render systems: sprites drawn after the ground tile pass.
    private static readonly Group<int> _groundRenderSystems = new(
        "GroundRenderSystems",
        new SpriteRenderSystem(GameContext.Instance.World)
    );

    // Fringe-layer render systems: effects drawn after the fringe tile pass.
    private static readonly Group<int> _fringeRenderSystems = new(
        "FringeRenderSystems",
        new TextRenderSystem(GameContext.Instance.World),
        new ScrollingOverlayRenderSystem(GameContext.Instance.World)
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

        // Ground layer
        _mapRenderer.DrawPanorama();
        _mapRenderer.DrawLayer((byte)Layer.Ground);
        _groundRenderSystems.Update(0);

        for (byte i = 0; i < _context.CurrentMap.Npc.Length; i++)
            if (_context.CurrentMap.Npc[i].Data != null)
                _npcRenderer.DrawNpc(_context.CurrentMap.Npc[i]);

        for (byte i = 0; i < Player.List.Count; i++)
            if (Player.List[i] != Player.Me)
                if (Player.List[i].MapInstance == Player.Me.MapInstance)
                    _playerRenderer.PlayerCharacter(Player.List[i]);

        _playerRenderer.PlayerCharacter(Player.Me);

        // Foreground layers and effects
        _mapRenderer.DrawLayer((byte)Layer.Fringe);
        _mapRenderer.DrawWeather();
        _fringeRenderSystems.Update(0);
        _mapRenderer.DrawMapName();

        _uiRenderer.DrawParty();

        // FPS/Latency overlays — these are world-space but near-origin so they work fine here.
        if (Options.Fps) _renderer.DrawText("FPS: " + Loop.Fps, 176, 7, Color.White);
        if (Options.Latency) _renderer.DrawText("Latency: " + NetworkClient.Latency, 176, 19, Color.White);
    }
}
