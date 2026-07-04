using CryBits.Definitions.Catalog;
using CryBits.Host;
using CryBits.Host.Core;
using CryBits.Host.Hosting;
using CryBits.Host.Ingress;
using CryBits.Host.Network;
using CryBits.Host.Network.Senders;
using CryBits.Host.Replication;
using CryBits.Host.Services;
using CryBits.Host.Services.Party;
using CryBits.Host.Services.Trade;
using CryBits.Persistence;
using CryBits.Persistence.Repositories;
using CryBits.Protocol.Serialization;
using CryBits.Server;
using CryBits.Server.Commands;
using CryBits.Simulation.Core;
using CryBits.Simulation.Intents;
using CryBits.Transport.Abstractions;
using CryBits.Transport.Udp;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.SQLite;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;
using ZLogger;

IntentRegistry.Register<MoveIntent>(1);
IntentRegistry.Register<AttackIntent>(2);
IntentRegistry.Register<AddPointIntent>(3);
IntentRegistry.Register<CollectItemIntent>(4);
IntentRegistry.Register<DropItemIntent>(5);
IntentRegistry.Register<InventorySwapIntent>(6);
IntentRegistry.Register<InventoryUseIntent>(7);
IntentRegistry.Register<EquipmentRemoveIntent>(8);
IntentRegistry.Register<HotbarAddIntent>(9);
IntentRegistry.Register<HotbarSwapIntent>(10);
IntentRegistry.Register<HotbarUseIntent>(11);
IntentRegistry.Register<ChatMessageIntent>(12);
IntentRegistry.Register<PartyInviteIntent>(13);
IntentRegistry.Register<PartyAcceptIntent>(14);
IntentRegistry.Register<PartyDeclineIntent>(15);
IntentRegistry.Register<PartyLeaveIntent>(16);
IntentRegistry.Register<TradeInviteIntent>(17);
IntentRegistry.Register<TradeAcceptIntent>(18);
IntentRegistry.Register<TradeDeclineIntent>(19);
IntentRegistry.Register<TradeLeaveIntent>(20);
IntentRegistry.Register<TradeOfferIntent>(21);
IntentRegistry.Register<TradeOfferStateIntent>(22);
IntentRegistry.Register<ShopBuyIntent>(23);
IntentRegistry.Register<ShopSellIntent>(24);
IntentRegistry.Register<ShopCloseIntent>(25);

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((ctx, services) =>
{
    services.AddLogging(logging =>
    {
        logging.AddConfiguration(ctx.Configuration.GetSection("Logging"));
        logging.AddCryBitsLogging(enableRollingFile: true);
    });

    services.AddSingleton<DefinitionCatalog>();

    // Database connection — single instance, WAL mode
    services.AddSingleton(_ =>
    {
        Directories.Database.Directory!.Create();
        var conn = new SqliteConnection($"Data Source={Directories.Database.FullName}");
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "PRAGMA journal_mode=WAL;";
        cmd.ExecuteNonQuery();
        return conn;
    });
    services.AddSingleton(sp => new DataConnection(
        new DataOptions().UseSQLite(sp.GetRequiredService<SqliteConnection>().ConnectionString, SQLiteProvider.Microsoft)));

    // Repositories
    services.AddSingleton<AccountRepository>();
    services.AddSingleton<CharacterRepository>();
    services.AddSingleton<ContentRepository>();
    services.AddSingleton<MapRepository>();
    services.AddSingleton<DataLoader>();

    services.AddSingleton<ITransport>(_ => new UdpTransport());

    services.AddSingleton<World>();
    services.AddSingleton(sp => sp.GetRequiredService<World>().Entities);
    services.AddSingleton<SessionManager>();
    services.AddSingleton(_ => HostPipelineBuilder.Build());
    services.AddSingleton<PackageSender>();
    services.AddSingleton<WorldHost>();
    services.AddSingleton(sp => sp.GetRequiredService<WorldHost>().IntentFunnel);
    services.AddSingleton<WorldInitializer>();

    services.AddSingleton<PacketDispatcher>();

    services.AddSingleton<AccountSender>();
    services.AddSingleton<AuthSender>();
    services.AddSingleton<ChatSender>();
    services.AddSingleton<ContentSender>();
    services.AddSingleton<ContentSender>();

    services.AddSingleton<TradeService>();
    services.AddSingleton<PartyService>();
    services.AddSingleton<AuthService>();
    services.AddSingleton<CharacterService>();
    services.AddSingleton<ContentService>();
    services.AddSingleton<KeyframeEncoder>();
    services.AddSingleton<EventFanout>();
    services.AddSingleton<InterestManager>();
    services.AddSingleton<KeyframeReplicator>();
    services.AddSingleton<IntentIngress>();

    services.AddSingleton<CommandDispatcher>();

    services.AddSingleton<object>(sp => sp.GetRequiredService<AuthService>());
    services.AddSingleton<object>(sp => sp.GetRequiredService<CharacterService>());
    services.AddSingleton<object>(sp => sp.GetRequiredService<ContentService>());
    services.AddSingleton<object>(sp => sp.GetRequiredService<IntentIngress>());

    services.AddHostedService<Server>();
});

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
ServerContext.LoggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown";
logger.ZLogInformation($"CryBits Server v{version} starting");

var host = app.Services.GetRequiredService<WorldHost>();
ServerContext.Host = host;
ServerContext.Catalog = app.Services.GetRequiredService<DefinitionCatalog>();

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    logger.ZLogInformation($"Server shutting down via Ctrl+C");
    cts.Cancel();
};
AppDomain.CurrentDomain.ProcessExit += (_, _) => cts.Cancel();
AppDomain.CurrentDomain.UnhandledException += (_, e) =>
{
    if (e.ExceptionObject is Exception ex)
        logger.ZLogCritical(ex, $"Unhandled exception in AppDomain");
};
TaskScheduler.UnobservedTaskException += (_, e) =>
{
    logger.ZLogError(e.Exception, $"Unobserved task exception");
    e.SetObserved();
};

var dispatcher = app.Services.GetRequiredService<CommandDispatcher>()
    .Register<DefineAccessCommand>()
    .Register<SeedCommand>();

var consoleThread = new Thread(() => ConsoleLoop.Run(dispatcher, cts.Token)) { IsBackground = true };
consoleThread.Start();

await app.RunAsync(cts.Token);
