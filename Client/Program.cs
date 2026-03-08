using System;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Persistence.Repositories;
using CryBits.Client.Graphics;
using CryBits.Client.Logic;
using CryBits.Client.Managers;
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
        var renderer = Renderer.Instance;
        var inputManager = InputManager.Instance;
        var networkClient = NetworkClient.Instance;
        var context = GameContext.Instance;
        var audioManager = AudioManager.Instance;
        var playerSender = PlayerSender.Instance;
        var partySender = PartySender.Instance;
        var tradeSender = TradeSender.Instance;
        var authSender = AuthSender.Instance;
        var accountSender = AccountSender.Instance;
        var shopSender = ShopSender.Instance;
        var mapSender = MapSender.Instance;
        var chatSender = ChatSender.Instance;

        new MenuScreen(networkClient, authSender, audioManager, accountSender, Graphics.Renderers.CharacterRenderer.Instance, context).Bind();
        new GameScreen(playerSender, Graphics.Renderers.EquipmentRenderer.Instance, Graphics.Renderers.CharacterRenderer.Instance, Graphics.Renderers.ItemRenderer.Instance, shopSender, tradeSender, partySender, audioManager, inputManager, context).Bind();
        Window.Bind();
        GameInput.Bind();

        networkClient.Init(context);

        PacketDispatcher.Register(new AuthHandler());
        PacketDispatcher.Register(new AccountHandler(audioManager, context));
        PacketDispatcher.Register(new PlayerHandler(context));
        PacketDispatcher.Register(new MapHandler(context, mapSender, audioManager));
        PacketDispatcher.Register(new NpcHandler(context));
        PacketDispatcher.Register(new ChatHandler());
        PacketDispatcher.Register(new PartyHandler(partySender, context));
        PacketDispatcher.Register(new TradeHandler(tradeSender, context));
        PacketDispatcher.Register(new ShopHandler());
        PacketDispatcher.Register(new ClassHandler());
        PacketDispatcher.Register(new ItemHandler());
        audioManager.LoadSounds();

        Window.OpenMenu();

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
