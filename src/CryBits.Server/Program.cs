using CryBits.Definitions.Catalog;
using CryBits.Host;
using CryBits.Host.Core;
using CryBits.Host.Network;
using CryBits.Host.Network.Senders;
using CryBits.Host.Services;
using CryBits.Persistence;
using CryBits.Persistence.Repositories;
using CryBits.Server;
using CryBits.Server.Commands;
using CryBits.Simulation.Core;
using CryBits.Transport.Abstractions;
using CryBits.Transport.Udp;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.SQLite;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((ctx, services) =>
{
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
    services.AddSingleton<DataLoader>();

    services.AddSingleton<ITransport>(_ => new UdpTransport());

    services.AddSingleton<World>();
    services.AddSingleton(sp => sp.GetRequiredService<World>().Entities);
    services.AddSingleton<SessionManager>();
    services.AddSingleton(sp => HostPipelineBuilder.Build(sp.GetRequiredService<DefinitionCatalog>()));
    services.AddSingleton<PackageSender>();
    services.AddSingleton<WorldHost>();
    services.AddSingleton<WorldInitializer>();

    services.AddSingleton<PacketDispatcher>();

    services.AddSingleton<AccountSender>();
    services.AddSingleton<AuthSender>();
    services.AddSingleton<ChatSender>();
    services.AddSingleton<ClassSender>();
    services.AddSingleton<CombatSender>();
    services.AddSingleton<ItemSender>();
    services.AddSingleton<MapSender>();
    services.AddSingleton<NpcSender>();
    services.AddSingleton<PartySender>();
    services.AddSingleton<PlayerSender>();
    services.AddSingleton<ShopSender>();
    services.AddSingleton<TradeSender>();

    services.AddSingleton<AuthService>();
    services.AddSingleton<CharacterService>();
    services.AddSingleton<PlayerService>();
    services.AddSingleton<ChatService>();
    services.AddSingleton<PartyService>();
    services.AddSingleton<TradeService>();
    services.AddSingleton<ShopService>();
    services.AddSingleton<EditorService>();
    services.AddSingleton<ReplicationService>();

    services.AddSingleton<CommandDispatcher>();

    services.AddSingleton<object>(sp => sp.GetRequiredService<AuthService>());
    services.AddSingleton<object>(sp => sp.GetRequiredService<CharacterService>());
    services.AddSingleton<object>(sp => sp.GetRequiredService<PlayerService>());
    services.AddSingleton<object>(sp => sp.GetRequiredService<ChatService>());
    services.AddSingleton<object>(sp => sp.GetRequiredService<PartyService>());
    services.AddSingleton<object>(sp => sp.GetRequiredService<TradeService>());
    services.AddSingleton<object>(sp => sp.GetRequiredService<ShopService>());
    services.AddSingleton<object>(sp => sp.GetRequiredService<EditorService>());

    services.AddHostedService<Server>();
});

var app = builder.Build();

var host = app.Services.GetRequiredService<WorldHost>();
ServerContext.Host = host;
ServerContext.Catalog = app.Services.GetRequiredService<DefinitionCatalog>();

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    Console.WriteLine("\r\n[Shutting down...]");
    cts.Cancel();
};
AppDomain.CurrentDomain.ProcessExit += (_, _) => cts.Cancel();
AppDomain.CurrentDomain.UnhandledException += (_, e) =>
    Console.WriteLine($"[Global Error] Unhandled exception: {e.ExceptionObject}");
TaskScheduler.UnobservedTaskException += (_, e) =>
{
    Console.WriteLine($"[Global Error] Unobserved task exception: {e.Exception}");
    e.SetObserved();
};

var dispatcher = new CommandDispatcher()
    .Register<DefineAccessCommand>()
    .Register<SeedCommand>();

var consoleThread = new Thread(() => ConsoleLoop.Run(dispatcher, cts.Token)) { IsBackground = true };
consoleThread.Start();

await app.RunAsync(cts.Token);
