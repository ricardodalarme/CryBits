using CryBits.Host;
using CryBits.Host.Core;
using CryBits.Host.Network;
using CryBits.Host.Services;
using CryBits.Persistence;
using CryBits.Transport.Abstractions;
using LinqToDB.Data;
using Microsoft.Extensions.Hosting;

namespace CryBits.Server;

internal sealed class Server(
    ITransport transport,
    WorldHost host,
    DataLoader dataLoader,
    PacketDispatcher dispatcher,
    WorldInitializer worldInitializer,
    ReplicationService replicationService,
    CharacterService characterService,
    DataConnection dataConnection,
    IEnumerable<object> packetHandlers) : IHostedService
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

        Console.WriteLine("[Starting]");

        Directories.Create();
        Console.WriteLine("Directories created.");

        SchemaBootstrap.EnsureCreated(dataConnection);
        Console.WriteLine("Database schema ensured.");

        Console.WriteLine("Creating world.");
        dataLoader.LoadAll();
        worldInitializer.Initialize();

        var config = Definitions.Globals.Config;
        transport.Start(config.Port, config.GameName, config.MaxPlayers);
        Console.WriteLine("Network started. Port: " + Definitions.Globals.Config.Port);

        host.Pipeline.AddSystem(replicationService);

        foreach (var handler in packetHandlers)
            dispatcher.Register(handler);
        Console.WriteLine($"PacketDispatcher: {dispatcher.Count} services registered.");

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
            var entity = host.Entities.Get(entityId);
            if (entity == null) continue;
            characterService.Leave(entityId);
        }

        _cts?.Cancel();
        transport.Stop();
        return Task.CompletedTask;
    }

    private void OnSessionConnected(Guid sessionId)
    {
        host.Sessions.Add(new Session(sessionId));
    }

    private void OnSessionDisconnected(Guid sessionId)
    {
        var session = host.Sessions.Find(s => s.Id == sessionId);
        if (session?.Character is { } characterId)
            characterService.Leave(characterId);
        if (session != null)
            host.Sessions.Remove(session);
    }

    private void OnSessionDataReceived(Guid sessionId, byte[] data)
    {
        var session = host.Sessions.Find(s => s.Id == sessionId);
        if (session != null)
            dispatcher.Dispatch(session, data);
    }
}
