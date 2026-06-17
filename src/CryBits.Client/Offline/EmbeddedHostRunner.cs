using CryBits.Client.Framework.Network;
using CryBits.Definitions.Catalog;
using CryBits.Host;
using CryBits.Host.Core;
using CryBits.Host.Network;
using CryBits.Host.Network.Senders;
using CryBits.Host.Services;
using CryBits.Persistence;
using CryBits.Persistence.Repositories;
using CryBits.Simulation.Core;
using CryBits.Transport.Transports;
using LinqToDB;
using LinqToDB.Data;
using Microsoft.Data.Sqlite;

namespace CryBits.Client.Offline;

public sealed class EmbeddedHostRunner : IDisposable
{
    private WorldHost? _host;
    private CancellationTokenSource? _cts;

    public Connection ClientConnection { get; private set; } = null!;

    public void Start()
    {
        var pair = new LoopbackPair();
        var catalog = DefinitionCatalog.Instance;
        var baseDir = AppContext.BaseDirectory;

        // Load content definitions
        var contentRepo = new ContentRepository();
        var dataLoader = new DataLoader(contentRepo, catalog);
        dataLoader.LoadAll();

        // SQLite database for account/character persistence
        var dbPath = Path.Combine(baseDir, "Data", "crybits.db");
        var conn = new SqliteConnection($"Data Source={dbPath}");
        conn.Open();
        var db = new DataConnection(new DataOptions().UseSQLiteMicrosoft(conn.ConnectionString));
        SchemaBootstrap.EnsureCreated(db);

        var accountRepo = new AccountRepository(db);
        var charRepo = new CharacterRepository(db);

        // Build simulation host
        var simulation = new World();
        var sessions = new SessionManager();
        var packageSender = new PackageSender(pair.Server, sessions, simulation.Entities);
        var pipeline = HostPipelineBuilder.Build(catalog);
        _host = new WorldHost(pair.Server, simulation, pipeline, sessions, packageSender);
        pair.Server.Start(0);

        new WorldInitializer(_host, catalog).Initialize();

        // Create an instance-based dispatcher and wire all host services
        var hostDispatcher = new CryBits.Host.Network.PacketDispatcher();
        var ps = _host.PackageSender;
        var es = _host.Entities;
        var ss = _host.Sessions;

        var authSender = new AuthSender(ps, pair.Server);
        var mapSender = new MapSender(ps, catalog, ss, es);
        var itemSender = new ItemSender(ps, catalog);
        var shopSender = new ShopSender(ps, catalog);
        var classSender = new ClassSender(ps, catalog);
        var npcSender = new NpcSender(ps, catalog, es);
        var accountSenderHost = new AccountSender(ps);
        var playerSender = new PlayerSender(ps, es);
        var chatSender = new ChatSender(ps, es);
        var combatSender = new CombatSender(ps);

        hostDispatcher.Register(new AuthService(
            authSender, mapSender, itemSender, shopSender, classSender, npcSender,
            accountSenderHost, accountRepo, charRepo, _host));

        hostDispatcher.Register(new CharacterService(
            charRepo, authSender, playerSender, itemSender, npcSender,
            shopSender, mapSender, accountSenderHost, classSender, chatSender, catalog, _host));

        hostDispatcher.Register(new PlayerService(_host));
        hostDispatcher.Register(new ChatService(_host));
        hostDispatcher.Register(new PartyService(_host));
        hostDispatcher.Register(new TradeService(_host));
        hostDispatcher.Register(new ShopService(_host));

        _host.Pipeline.AddSystem(new ReplicationService(
            playerSender, npcSender, mapSender, combatSender, chatSender, ss));

        // Start server tick loop
        _cts = new CancellationTokenSource();
        var ct = _cts.Token;
        ThreadPool.QueueUserWorkItem(_ => _host!.StartTickLoop(ct));

        // Wire transport events to session management and packet dispatch
        pair.Server.OnConnected += id => _host.Sessions.Add(new Session(id));
        pair.Server.OnDisconnected += id =>
        {
            var s = _host.Sessions.Find(x => x.Id == id);
            if (s != null) _host.Sessions.Remove(s);
        };
        pair.Server.OnDataReceived += (id, data) =>
        {
            var s = _host.Sessions.Find(x => x.Id == id);
            if (s != null) hostDispatcher.Dispatch(s, data);
        };

        // Create client-side connection over the loopback transport
        ClientConnection = new Connection(pair.Client);
    }

    public void Stop()
    {
        _cts?.Cancel();
    }

    public void Dispose()
    {
        Stop();
        _cts?.Dispose();
        _host = null;
    }
}
