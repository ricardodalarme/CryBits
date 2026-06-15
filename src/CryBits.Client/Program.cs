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
using CryBits.Host;
using CryBits.Host.Core;
using CryBits.Host.Network;
using CryBits.Host.Persistence.Repositories;
using CryBits.Host.Services;
using CryBits.Persistence.Stores;
using CryBits.Simulation.Core;
using CryBits.Transport.Transports;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static CryBits.Definitions.Globals;
using CrybitsHost = CryBits.Host;

namespace CryBits.Client;

internal static class Program
{
    public static bool Working = true;

    private static CancellationTokenSource _cts = new();
    private static Task? _hostTask;
    private static Connection? _connection;
    private static WorldHost? _offlineHost;

    [STAThread]
    private static void Main(string[] args)
    {
        CryBits.Host.Persistence.Directories.Create();

        ToolsRepository.Instance.Read();
        OptionsRepository.Read();

        Renderer.Instance.Init();

        if (args.Contains("--offline"))
        {
            var pair = new LoopbackPair();
            var catalog = DefinitionCatalog.Instance;
            var offlineContentStore = new FileContentStore(new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "Data")));
            var settingsRepo = new SettingsRepository();
            var dataLoader = new CrybitsHost.Persistence.DataLoader(settingsRepo, offlineContentStore, catalog);
            dataLoader.LoadAll();

            var simulation = new World();
            var sessions = new SessionManager();
            var packageSender = new PackageSender(pair.Server, sessions, simulation.Entities);
            var pipeline = HostPipelineBuilder.Build(catalog);
            var host = new WorldHost(pair.Server, simulation, pipeline, sessions, packageSender);
            pair.Server.Start(0);

            var worldInitializer = new WorldInitializer(host, catalog);
            worldInitializer.Initialize();

            var hostDispatcher = new CrybitsHost.Network.PacketDispatcher();
            var ps = host.PackageSender;
            var es = host.Entities;
            var ss = host.Sessions;

            var authSender = new CrybitsHost.Network.Senders.AuthSender(ps, pair.Server);
            var mapSender = new CrybitsHost.Network.Senders.MapSender(ps, catalog, ss, es);
            var itemSender = new CrybitsHost.Network.Senders.ItemSender(ps, catalog);
            var shopSender = new CrybitsHost.Network.Senders.ShopSender(ps, catalog);
            var classSender = new CrybitsHost.Network.Senders.ClassSender(ps, catalog);
            var npcSender = new CrybitsHost.Network.Senders.NpcSender(ps, catalog, es);
            var accountSenderHost = new CrybitsHost.Network.Senders.AccountSender(ps);
            var playerSender = new CrybitsHost.Network.Senders.PlayerSender(ps, es);
            var chatSender = new CrybitsHost.Network.Senders.ChatSender(ps, es);
            var combatSender = new CrybitsHost.Network.Senders.CombatSender(ps);
            var accountRepo = new AccountRepository();
            var charRepo = new CharacterRepository();

            hostDispatcher.Register(new AuthService(
                authSender, mapSender, itemSender, shopSender, classSender, npcSender,
                accountSenderHost, accountRepo, host));

            hostDispatcher.Register(new CharacterService(
                charRepo, accountRepo, authSender, playerSender, itemSender, npcSender,
                shopSender, mapSender, accountSenderHost, classSender, chatSender, catalog, host));

            hostDispatcher.Register(new PlayerService(host));
            hostDispatcher.Register(new ChatService(host));
            hostDispatcher.Register(new PartyService(host));
            hostDispatcher.Register(new TradeService(host));
            hostDispatcher.Register(new ShopService(host));

            host.Pipeline.AddSystem(new ReplicationService(
                playerSender, npcSender, mapSender, combatSender, chatSender, ss));

            _cts = new CancellationTokenSource();
            _offlineHost = host;
            _hostTask = Task.Run(() => host.StartTickLoop(new CancellationToken()));

            pair.Server.OnConnected += id => host.Sessions.Add(new Session(id));
            pair.Server.OnDisconnected += id =>
            {
                var session = host.Sessions.Find(s => s.Id == id);
                if (session != null) host.Sessions.Remove(session);
            };
            pair.Server.OnDataReceived += (id, data) =>
            {
                var session = host.Sessions.Find(s => s.Id == id);
                if (session != null) hostDispatcher.Dispatch(session, data);
            };

            _connection = new Connection(pair.Client);
        }
        else
        {
            var clientTransport = new UdpClientTransport();
            clientTransport.Connect("localhost", Config.Port, Config.GameName);
            _connection = new Connection(clientTransport);
        }

        _connection.Start(onDisconnected: Leave);

        new MenuScreen().Bind();
        new GameScreen().Bind();
        Window.Instance.Bind();
        GameInput.Instance.Bind();

        var context = GameContext.Instance;
        var audioManager = AudioManager.Instance;

        var contentStore = new FileContentStore(new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "Data")));
        var cat = DefinitionCatalog.Instance;

        Client.Framework.Network.PacketDispatcher.Register(new AuthHandler(cat));
        Client.Framework.Network.PacketDispatcher.Register(new AccountHandler(audioManager, context, cat));
        Client.Framework.Network.PacketDispatcher.Register(new PlayerHandler(context, cat));
        Client.Framework.Network.PacketDispatcher.Register(new MapHandler(context, MapSender.Instance, audioManager, cat, contentStore));
        Client.Framework.Network.PacketDispatcher.Register(new NpcHandler(context, cat));
        Client.Framework.Network.PacketDispatcher.Register(new CombatHandler(context));
        Client.Framework.Network.PacketDispatcher.Register(new ChatHandler(Chat.Instance));
        Client.Framework.Network.PacketDispatcher.Register(new PartyHandler(PartySender.Instance, context));
        Client.Framework.Network.PacketDispatcher.Register(new TradeHandler(TradeSender.Instance, context, cat));
        Client.Framework.Network.PacketDispatcher.Register(new ShopHandler(cat));
        Client.Framework.Network.PacketDispatcher.Register(new ClassHandler(cat));
        Client.Framework.Network.PacketDispatcher.Register(new ItemHandler(cat));
        AudioManager.Instance.LoadSounds();

        Window.Instance.OpenMenu();

        GameLoop.Instance.Init();
    }

    private static void Leave()
    {
        GameContext.Instance.Reset();
        Window.Instance.OpenMenu();
    }

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
