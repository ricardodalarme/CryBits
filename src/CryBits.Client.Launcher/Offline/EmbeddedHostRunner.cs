using CryBits.Client.Framework.Network;
using CryBits.Definitions.Catalog;
using CryBits.Host;
using CryBits.Host.Core;
using CryBits.Host.Network;
using CryBits.Host.Network.Handlers;
using CryBits.Host.Network.Senders;
using CryBits.Host.Replication;
using CryBits.Host.Scheduling;
using CryBits.Host.Services;
using CryBits.Persistence;
using CryBits.Persistence.Repositories;
using CryBits.Protocol.Serialization;
using CryBits.Simulation;
using CryBits.Simulation.Core;
using CryBits.Transport.Transports;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.SQLite;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using static CryBits.Definitions.Globals;

namespace CryBits.Client.Launcher.Offline;

internal sealed class SilentLogger<T> : ILogger<T>
{
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
    public bool IsEnabled(LogLevel logLevel) => false;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
    }
}

public sealed class EmbeddedHostRunner : IDisposable
{
    private WorldHost? _host;
    private CancellationTokenSource? _cts;

    public Connection ClientConnection { get; private set; } = null!;

    public void Start()
    {
        var pair = new LoopbackPair();
        var catalog = new DefinitionCatalog();
        var baseDir = AppContext.BaseDirectory;

        // Load content definitions
        var contentRepo = new ContentRepository();
        var mapRepo = new MapRepository();
        var dataLoader = new DataLoader(contentRepo, mapRepo, catalog, new SilentLogger<DataLoader>());
        dataLoader.LoadAll();

        // SQLite database for account/character persistence
        var dbPath = Path.Combine(baseDir, "Data", "crybits.db");
        var conn = new SqliteConnection($"Data Source={dbPath}");
        conn.Open();
        var db = new DataConnection(new DataOptions().UseSQLite(conn.ConnectionString, SQLiteProvider.Microsoft));
        SchemaBootstrap.EnsureCreated(db);

        var accountRepo = new AccountRepository(db);
        var charRepo = new CharacterRepository(db);

        // Build simulation host
        RegisterComponentTypes();
        var simulation = new World(catalog);
        var sessions = new SessionManager();
        var packageSender = new PackageSender(pair.Server, sessions, simulation.Entities);

        var contentSender = new ContentSender(packageSender, catalog);
        var chatSender = new ChatSender(packageSender, simulation.Entities);

        var deltaEncoder = new DeltaEncoder(simulation);
        var interestManager = new InterestManager(simulation);

        var eventFanout = new EventFanout(sessions, chatSender, contentSender, pair.Server);

        // Build pipeline (includes DeltaReplicator as the final system)
        var pipeline = HostPipelineBuilder.Build(new DeltaReplicator(
            simulation, sessions, deltaEncoder, eventFanout, pair.Server, interestManager));

        _host = new WorldHost(pair.Server, simulation, pipeline, sessions, packageSender,
            new SilentLogger<TickDriver>());
        pair.Server.Start(0, Config.GameName, 1);

        new WorldInitializer(_host).Initialize();

        // Create an instance-based dispatcher and wire all host services
        var hostDispatcher = new Host.Network.PacketDispatcher(new SilentLogger<Host.Network.PacketDispatcher>());
        var ps = _host.PackageSender;
        var ss = _host.Sessions;

        var authSender = new AuthSender(ps, pair.Server);
        var accountSenderHost = new AccountSender(ps);

        hostDispatcher.Register(new AuthService(
            authSender, contentSender,
            accountSenderHost, accountRepo, charRepo, _host, new SilentLogger<AuthService>()));

        var partyService = new Host.Services.Party.PartyService(_host.IntentFunnel, ps, chatSender, ss, simulation);

        hostDispatcher.Register(new CharacterService(
            new SilentLogger<CharacterService>(),
            charRepo, authSender, contentSender,
            accountSenderHost, chatSender, catalog, _host,
            deltaEncoder, interestManager, pair.Server,
            partyService));

        hostDispatcher.Register(new AckHandler());

        var tradeService = new Host.Services.Trade.TradeService(_host.IntentFunnel, ps, ss, simulation);
        var intentIngress = new Host.Ingress.IntentIngress(_host.IntentFunnel, tradeService, partyService,
            new SilentLogger<Host.Ingress.IntentIngress>());
        hostDispatcher.Register(intentIngress);

        // Start server tick loop
        _cts = new CancellationTokenSource();
        var ct = _cts.Token;
        ThreadPool.QueueUserWorkItem(_ => _host.StartTickLoop(ct));

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

    private static void RegisterComponentTypes()
    {
        ComponentTypes.RegisterDefault();
        ComponentTypeRegistry.Register<Components.NetworkId>(18);
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
