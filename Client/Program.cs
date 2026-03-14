using System;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Network;
using CryBits.Client.Framework.Persistence.Repositories;
using CryBits.Client.Graphics;
using CryBits.Client.Logic;
using CryBits.Client.Network;
using CryBits.Client.Network.Handlers;
using CryBits.Client.Network.Senders;
using CryBits.Client.UI;
using CryBits.Client.UI.Game;
using CryBits.Client.UI.Menu;
using CryBits.Client.Worlds;

namespace CryBits.Client;

internal static class Program
{
    /// <summary>
    /// Indicates whether the application main loop is running.
    /// </summary>
    public static bool Working = true;

    [STAThread]
    private static void Main()
    {
        Directories.Create();

        ToolsRepository.Read();
        OptionsRepository.Read();

        // Window must be created before any event bindings that require it.
        Renderer.Instance.Init();

        // Register all input and UI event handlers.
        new MenuScreen().Bind();
        new GameScreen().Bind();
        Window.Instance.Bind();
        GameInput.Instance.Bind();

        NetworkClient.Instance.Init();
        var context = GameContext.Instance;
        var audioManager = AudioManager.Instance;

        PacketDispatcher.Register(new AuthHandler());
        PacketDispatcher.Register(new AccountHandler(audioManager, context));
        PacketDispatcher.Register(new PlayerHandler(context));
        PacketDispatcher.Register(new MapHandler(context, MapSender.Instance, audioManager));
        PacketDispatcher.Register(new NpcHandler(context));
        PacketDispatcher.Register(new ChatHandler(Chat.Instance));
        PacketDispatcher.Register(new PartyHandler(PartySender.Instance, context));
        PacketDispatcher.Register(new TradeHandler(TradeSender.Instance, context));
        PacketDispatcher.Register(new ShopHandler());
        PacketDispatcher.Register(new ClassHandler());
        PacketDispatcher.Register(new ItemHandler());
        AudioManager.Instance.LoadSounds();

        Window.Instance.OpenMenu();

        GameLoop.Instance.Init();
    }

    /// <summary>
    /// Disconnects from the server and exits the application.
    /// </summary>
    public static void Close()
    {
        var waitTimer = Environment.TickCount;

        NetworkClient.Instance.Disconnect();

        while (NetworkClient.Instance.IsConnected() && Environment.TickCount <= waitTimer + 1000)
            NetworkClient.Instance.HandleData();

        Working = false;
        Environment.Exit(0);
    }
}
