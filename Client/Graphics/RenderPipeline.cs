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

internal static class RenderPipeline
{
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
    public static void Present()
    {
        Renderer.Instance.RenderWindow.Clear(Color.Black);

        InGame();

        // Restore the default view before drawing UI so it renders at fixed screen positions.
        CameraManager.Instance.BeginUIDraw();

        UIRenderer.Interface(Screen.Current?.Body);

        if (Screen.Current == Screens.Game) UIRenderer.Chat();

        Renderer.Instance.RenderWindow.Display();
    }

    private static void InGame()
    {
        if (Screen.Current != Screens.Game) return;

        // Update camera logic and apply the SFML view.
        // All subsequent draws happen in world-space coordinates.
        CameraManager.Instance.Update();
        CameraManager.Instance.BeginWorldDraw();

        // Ground layer
        MapRenderer.MapPanorama();
        MapRenderer.MapTiles((byte)Layer.Ground);
        _groundRenderSystems.Update(0);

        for (byte i = 0; i < GameContext.Instance.CurrentMap.Npc.Length; i++)
            if (GameContext.Instance.CurrentMap.Npc[i].Data != null)
                NpcRenderer.Npc(GameContext.Instance.CurrentMap.Npc[i]);

        for (byte i = 0; i < Player.List.Count; i++)
            if (Player.List[i] != Player.Me)
                if (Player.List[i].MapInstance == Player.Me.MapInstance)
                    PlayerRenderer.PlayerCharacter(Player.List[i]);

        PlayerRenderer.PlayerCharacter(Player.Me);

        // Foreground layers and effects
        MapRenderer.MapTiles((byte)Layer.Fringe);
        MapRenderer.MapWeather();
        _fringeRenderSystems.Update(0);
        MapRenderer.MapName();

        UIRenderer.Party();

        // FPS/Latency overlays — these are world-space but near-origin so they work fine here.
        if (Options.Fps) Renderer.Instance.DrawText("FPS: " + Loop.Fps, 176, 7, Color.White);
        if (Options.Latency) Renderer.Instance.DrawText("Latency: " + Socket.Latency, 176, 19, Color.White);
    }
}
