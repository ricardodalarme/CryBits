using System;
using System.Threading;
using System.Threading.Tasks;
using CryBits.Entities.Map;
using CryBits.Server.Entities;
using CryBits.Server.Logic;
using CryBits.Server.Network;
using CryBits.Server.Persistence;
using CryBits.Server.Persistence.Repositories;
using CryBits.Server.World;

namespace CryBits.Server;

internal static class Program
{
    private static async Task Main()
    {
        Console.Title = "Server";
        Logo();
        Console.WriteLine("[Starting]");

        using var cts = new CancellationTokenSource();

        // Hook console shutdown handlers (cross-platform)
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            Console.WriteLine("\r\n[Shutting down...]");
            cts.Cancel();
        };
        AppDomain.CurrentDomain.ProcessExit += (_, _) => cts.Cancel();

        // Ensure required directories exist.
        Directories.Create();
        Console.WriteLine("Directories created.");

        // Load all game data.
        DataLoader.LoadAll();

        // Create world
        Console.WriteLine("Creating world.");
        _ = new GameWorld();

        // Create temporary maps.
        Console.WriteLine("Creating temporary maps.");
        foreach (var map in Map.List.Values) MapInstance.CreateTemporary(map, true);

        // Initialize network sockets.
        Socket.Init();
        Console.WriteLine("Network started. Port: " + Globals.Config.Port);

        // Register all [PacketHandler] methods before accepting connections.
        PacketDispatcher.Register();

        Console.WriteLine("\r\n" + "Server started. Type 'help' to see the commands." + "\r\n");

        // Start command loop on background thread.
        var consoleThread = new Thread(() => Loop.Commands(cts.Token)) { IsBackground = true };
        consoleThread.Start();

        // Start main loop and wait for cancellation.
        try
        {
            await Loop.MainAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
        }

        PerformShutdown();
    }

    private static void PerformShutdown()
    {
        // Save character data for all connected players.
        for (var i = 0; i < Account.List.Count; i++)
            if (Account.List[i].IsPlaying)
                CharacterRepository.Write(Account.List[i]);

        // Stop network device.
        Socket.Device.Stop();
    }

    private static void Logo()
    {
        Console.WriteLine(@"  ______              _____     _
 |   ___|            |     \   | |
 |  |     _ ____   _ |   __/ _ | |_  ___
 |  |    | '__/\\ // |   \_ | || __|/ __|
 |  |___ | |    | |  |     \| || |_ \__ \
 |______||_|    |_|  |_____/|_| \__||___/
                          2D orpg engine" + "\r\n");
    }
}
