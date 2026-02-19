using CryBits.Client.Entities;
using CryBits.Client.Entities.TempMap;
using CryBits.Client.Framework;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Logic;
using CryBits.Client.Network;
using CryBits.Enums;
using static CryBits.Client.Logic.Camera;
using Color = SFML.Graphics.Color;

namespace CryBits.Client.Graphics;

internal static class RenderPipeline
{
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
        MapRenderer.MapBlood();
        MapRenderer.MapItems();

        for (byte i = 0; i < TempMap.Current.Npc.Length; i++)
            if (TempMap.Current.Npc[i].Data != null)
                NpcRenderer.Npc(TempMap.Current.Npc[i]);

        for (byte i = 0; i < Player.List.Count; i++)
            if (Player.List[i] != Player.Me)
                if (Player.List[i].Map == Player.Me.Map)
                    PlayerRenderer.PlayerCharacter(Player.List[i]);

        PlayerRenderer.PlayerCharacter(Player.Me);

        // Foreground layers and effects
        MapRenderer.MapTiles((byte)Layer.Fringe);
        MapRenderer.MapWeather();
        MapRenderer.MapFog();
        MapRenderer.MapName();

        UIRenderer.Party();

        if (Options.Fps) Renders.DrawText("FPS: " + Loop.Fps, 176, 7, Color.White);
        if (Options.Latency) Renders.DrawText("Latency: " + Socket.Latency, 176, 19, Color.White);
    }
}
