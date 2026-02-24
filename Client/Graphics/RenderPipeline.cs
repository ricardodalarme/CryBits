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
using static CryBits.Client.Logic.Camera;
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
        Renders.RenderWindow.Clear(Color.Black);

        InGame();

        UIRenderer.Interface(Screen.Current?.Body);

        if (Screen.Current == Screens.Game) UIRenderer.Chat();

        Renders.RenderWindow.Display();
    }

    private static void InGame()
    {
        if (Screen.Current != Screens.Game) return;

        Update();

        // Layers and ground objects
        MapRenderer.MapPanorama();
        MapRenderer.MapTiles((byte)Layer.Ground);
        _groundRenderSystems.Update(0);

        for (byte i = 0; i < MapInstance.Current.Npc.Length; i++)
            if (MapInstance.Current.Npc[i].Data != null)
                NpcRenderer.Npc(MapInstance.Current.Npc[i]);

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

        if (Options.Fps) Renders.DrawText("FPS: " + Loop.Fps, 176, 7, Color.White);
        if (Options.Latency) Renders.DrawText("Latency: " + Socket.Latency, 176, 19, Color.White);
    }
}
