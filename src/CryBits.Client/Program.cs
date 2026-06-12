using CryBits.Definitions.Catalog;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Network;
using CryBits.Client.Framework.Persistence.Repositories;
using CryBits.Client.Graphics;
using CryBits.Persistence.Stores;
using System.IO;
using CryBits.Client.Logic;
using CryBits.Client.Network.Handlers;
using CryBits.Client.Network.Senders;
using CryBits.Client.UI;
using CryBits.Client.UI.Game;
using CryBits.Client.UI.Menu;
using CryBits.Client.Worlds;
using System;

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

        ToolsRepository.Instance.Read();
        OptionsRepository.Read();

        // Window must be created before any event bindings that require it.
        Renderer.Instance.Init();

        // Register all input and UI event handlers.
        new MenuScreen().Bind();
        new GameScreen().Bind();
        Window.Instance.Bind();
        GameInput.Instance.Bind();

        NetworkClient.Instance.Start(onDisconnected: Leave);
        var context = GameContext.Instance;
        var audioManager = AudioManager.Instance;
        var contentStore = new FileContentStore(new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "Data")));

        PacketDispatcher.Register(new AuthHandler(DefinitionCatalog.Instance));
        PacketDispatcher.Register(new AccountHandler(audioManager, context, DefinitionCatalog.Instance));
        PacketDispatcher.Register(new PlayerHandler(context, DefinitionCatalog.Instance));
        PacketDispatcher.Register(new MapHandler(context, MapSender.Instance, audioManager, DefinitionCatalog.Instance, contentStore));
        PacketDispatcher.Register(new NpcHandler(context, DefinitionCatalog.Instance));
        PacketDispatcher.Register(new CombatHandler(context));
        PacketDispatcher.Register(new ChatHandler(Chat.Instance));
        PacketDispatcher.Register(new PartyHandler(PartySender.Instance, context));
        PacketDispatcher.Register(new TradeHandler(TradeSender.Instance, context, DefinitionCatalog.Instance));
        PacketDispatcher.Register(new ShopHandler(DefinitionCatalog.Instance));
        PacketDispatcher.Register(new ClassHandler(DefinitionCatalog.Instance));
        PacketDispatcher.Register(new ItemHandler(DefinitionCatalog.Instance));
        AudioManager.Instance.LoadSounds();

        Window.Instance.OpenMenu();

        GameLoop.Instance.Init();
    }

    private static void Leave()
    {
        GameContext.Instance.Reset();
        Window.Instance.OpenMenu();
    }

    /// <summary>
    /// Disconnects from the server and exits the application.
    /// </summary>
    public static void Close()
    {
        var waitTimer = Environment.TickCount64;

        NetworkClient.Instance.Disconnect();

        while (NetworkClient.Instance.IsConnected() && Environment.TickCount64 <= waitTimer + 1000)
            NetworkClient.Instance.HandleData();

        Working = false;
        Environment.Exit(0);
    }
}
