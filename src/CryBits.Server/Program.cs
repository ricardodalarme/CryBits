using CryBits.Definitions.Catalog;
using CryBits.Host;
using CryBits.Host.Core;
using CryBits.Host.Network;
using CryBits.Host.Network.Senders;
using CryBits.Host.Persistence;
using CryBits.Host.Persistence.Repositories;
using CryBits.Host.Services;
using CryBits.Persistence.Stores;
using CryBits.Server;
using CryBits.Server.Commands;
using CryBits.Simulation.Core;
using CryBits.Transport.Abstractions;
using CryBits.Transport.Udp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((ctx, services) =>
{
    services.AddSingleton<DefinitionCatalog>();

    services.AddSingleton(_ =>
        new FileContentStore(new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "Data"))));

    services.AddSingleton<SettingsRepository>();
    services.AddSingleton<AccountRepository>();
    services.AddSingleton<CharacterRepository>();
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
ServerContext.AccountRepository = app.Services.GetRequiredService<AccountRepository>();

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
