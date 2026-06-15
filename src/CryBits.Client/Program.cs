using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Network;
using CryBits.Client.Framework.Network.Transport;
using CryBits.Client.Framework.Persistence.Repositories;
using CryBits.Client.Graphics;
using CryBits.Client.Logic;
using CryBits.Client.Network.Handlers;
using CryBits.Client.Network.Senders;
using CryBits.Client.UI;
using CryBits.Client.UI.Game;
using CryBits.Client.UI.Menu;
using CryBits.Client.Worlds;
using CryBits.Definitions.Catalog;
using CryBits.Host.Core;
using CryBits.Host.Scheduling;
using CryBits.Persistence.Stores;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static CryBits.Definitions.Globals;
using CryBits.Transport.Transports;

namespace CryBits.Client;

internal static class Program
{
    /// <summary>
    /// Indicates whether the application main loop is running.
    /// </summary>
    public static bool Working = true;

    private static CancellationTokenSource _cts = new();
    private static Task? _hostTask;
    private static Connection? _connection;

    [STAThread]
    private static void Main(string[] args)
    {
        Directories.Create();

        ToolsRepository.Instance.Read();
        OptionsRepository.Read();

        // Window must be created before any event bindings that require it.
        Renderer.Instance.Init();

        // Establish connection before registering views that depend on Connection.Instance.
        if (args.Contains("--offline"))
        {
            var pair = new LoopbackPair();
            var host = new WorldHost(pair.Server);
            pair.Server.Start(0);
            host.Initialize();
            host.RegisterDefaultServices();

            _cts = new CancellationTokenSource();
            _hostTask = Task.Run(() => TickDriver.Instance.MainAsync(_cts.Token));

            _connection = new Connection(pair.Client);
        }
        else
        {
            // Online mode
            var clientTransport = new UdpClientTransport();
            clientTransport.Connect("localhost", Config.Port, Config.GameName);
            _connection = new Connection(clientTransport);
        }

        _connection.Start(onDisconnected: Leave);

        // Register all input and UI event handlers (may reference Connection.Instance).
        new MenuScreen().Bind();
        new GameScreen().Bind();
        Window.Instance.Bind();
        GameInput.Instance.Bind();

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

        _connection?.Disconnect();
        _cts.Cancel();

        while (_connection?.IsConnected == true && Environment.TickCount64 <= waitTimer + 1000)
            _connection.Poll();

        Working = false;
        Environment.Exit(0);
    }
}
