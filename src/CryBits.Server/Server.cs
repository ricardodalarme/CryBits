using CryBits.Host;
using CryBits.Host.Core;
using CryBits.Host.Network;
using CryBits.Host.Services;
using CryBits.Persistence;
using CryBits.Simulation;
using CryBits.Transport.Abstractions;
using LinqToDB.Data;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ZLogger;

namespace CryBits.Server;

internal sealed class Server(
    ITransport transport,
    WorldHost host,
    DataLoader dataLoader,
    PacketDispatcher dispatcher,
    WorldInitializer worldInitializer,
    CharacterService characterService,
    DataConnection dataConnection,
    IEnumerable<object> packetHandlers,
    ILogger<Server> logger) : IHostedService
{
    private CancellationTokenSource? _cts;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Console.Title = "Server";
        Console.WriteLine(@"
  ______              _____     _
 |   ___|            |     \   | |
 |  |     _ ____   _ |   __/ _ | |_  ___
 |  |    | '__/\\ // |   \_ | || __|/ __|
 |  |___ | |    | |  |     \| || |_ \__ \
 |______||_|    |_|  |_____/|_| \__||___/
                           2D orpg engine" + "\r\n");

        logger.ZLogInformation($"Server starting");

        Directories.Create();
        logger.ZLogDebug($"Directories created");

        SchemaBootstrap.EnsureCreated(dataConnection);
        logger.ZLogInformation($"Database schema ensured");

        logger.ZLogInformation($"Creating world");
        ComponentTypes.RegisterDefault();
        dataLoader.LoadAll();
        worldInitializer.Initialize();

        var config = Definitions.Globals.Config;
        transport.Start(config.Port, config.GameName, config.MaxPlayers);
        logger.ZLogInformation($"Network started on port {config.Port}");

        foreach (var handler in packetHandlers)
            dispatcher.Register(handler);
        logger.ZLogDebug($"PacketDispatcher: {dispatcher.Count} services registered");

        transport.OnConnected += OnSessionConnected;
        transport.OnDisconnected += OnSessionDisconnected;
        transport.OnDataReceived += OnSessionDataReceived;

        Console.WriteLine("\r\nServer started. Type 'help' to see the commands.\r\n");

        _cts = new CancellationTokenSource();
        host.StartTickLoop(_cts.Token);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (var t in host.Sessions)
        {
            if (!t.IsPlaying) continue;
            var entityId = t.Character!.Value;
            var hasEntity = host.Entities.All.Contains(entityId);
            if (!hasEntity) continue;
            characterService.Leave(entityId);
        }

        _cts?.Cancel();
        transport.Stop();
        return Task.CompletedTask;
    }

    private void OnSessionConnected(Guid sessionId)
    {
        host.Sessions.Add(new Session(sessionId));
        logger.ZLogInformation($"Session {sessionId} connected");
    }

    private void OnSessionDisconnected(Guid sessionId)
    {
        var session = host.Sessions.Find(s => s.Id == sessionId);
        var characterId = session?.Character;
        if (characterId is { } cid)
            characterService.Leave(cid);
        if (session != null)
            host.Sessions.Remove(session);
        logger.ZLogInformation($"Session {sessionId} disconnected (character: {characterId?.Value ?? 0})");
    }

    private void OnSessionDataReceived(Guid sessionId, byte[] data)
    {
        var session = host.Sessions.Find(s => s.Id == sessionId);
        if (session != null)
            dispatcher.Dispatch(session, data);
    }
}
