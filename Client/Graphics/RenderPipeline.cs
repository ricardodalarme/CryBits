using CryBits.Client.ECS;
using CryBits.Client.ECS.Systems;
using CryBits.Client.Framework;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Logic;
using CryBits.Client.Network;
using Color = SFML.Graphics.Color;

namespace CryBits.Client.Graphics;

internal static class RenderPipeline
{
    // Render systems — instantiated once, reused every frame.
    private static readonly CameraSystem _cameraSystem = new();
    private static readonly MapRenderSystem _mapRenderer = new();
    private static readonly NpcRenderSystem _npcRenderer = new();
    private static readonly PlayerRenderSystem _playerRenderer = new();

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

        var ctx = GameContext.Instance;

        // Camera viewport must be updated before any drawing.
        _cameraSystem.Render(ctx);

        // Ground layers: panorama, tiles, blood splatters, dropped items.
        _mapRenderer.Render(ctx);

        // Characters rendered between ground and fringe layers.
        _npcRenderer.Render(ctx);
        _playerRenderer.Render(ctx);

        // Fringe layer, weather, fog and map name drawn on top of characters.
        _mapRenderer.RenderFringe(ctx);

        // Party HP bars overlay (HUD element rendered in world-space).
        UIRenderer.Party();

        if (Options.Fps) Renders.DrawText("FPS: " + Loop.Fps, 176, 7, Color.White);
        if (Options.Latency) Renders.DrawText("Latency: " + Socket.Latency, 176, 19, Color.White);
    }
}
