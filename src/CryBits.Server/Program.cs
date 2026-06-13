using CryBits.Definitions;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Characters;
using CryBits.Definitions.Common;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Host.Core;
using CryBits.Host.Logic;
using CryBits.Host.Network;
using CryBits.Host.Network.Handlers;
using CryBits.Host.Persistence;
using CryBits.Host.Persistence.Repositories;
using CryBits.Server.Commands;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Host.Systems.Npc;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

        // Global exception handlers to prevent crashing
        AppDomain.CurrentDomain.UnhandledException += (_, e) => Console.WriteLine($"[Global Error] Unhandled exception: {e.ExceptionObject}");
        TaskScheduler.UnobservedTaskException += (_, e) =>
        {
            Console.WriteLine($"[Global Error] Unobserved task exception: {e.Exception}");
            e.SetObserved();
        };

        // Ensure required directories exist.
        Directories.Create();
        Console.WriteLine("Directories created.");

        // Load all game data.
        DataLoader.Instance.LoadAll();

        // Create world
        Console.WriteLine("Creating world.");
        _ = new WorldHost();

        // Create temporary maps.
        Console.WriteLine("Creating map instances.");
        var world = WorldHost.Current;
        foreach (var map in DefinitionCatalog.Instance.Maps.Values)
        {
            var mapState = new MapState(map.Id, map);
            mapState.SpawnItems();
            world.Simulation.Maps.Add(map.Id, mapState);

            for (byte i = 0; i < map.Npc.Count; i++)
            {
                var npcData = DefinitionCatalog.Instance.Npcs.Get(map.Npc[i].NpcId);
                if (npcData == null) continue;

                var entityId = world.Entities.Create();
                var entityState = world.Entities.Get(entityId)!;

                entityState.Set(new NpcState
                {
                    Index = i,
                    NpcDefId = map.Npc[i].NpcId,
                    Alive = false,
                    TargetId = null,
                    SpawnTimer = 0,
                    AttackTimer = 0
                });

                entityState.Set(new Position
                {
                    X = map.Npc[i].X,
                    Y = map.Npc[i].Y,
                    Direction = Direction.Down,
                    MapId = mapState.Id
                });

                entityState.Set(new Vitals
                {
                    Hp = npcData.Vital[(byte)Vital.Hp],
                    Mp = npcData.Vital[(byte)Vital.Mp],
                    MaxHp = npcData.Vital[(byte)Vital.Hp],
                    MaxMp = npcData.Vital[(byte)Vital.Mp]
                });

                entityState.Set(new CombatState());
                entityState.Set(new NpcTag());

                mapState.NpcIds.Add(entityId);
                NpcBrainSystem.Instance.Spawn(WorldHost.Current.Simulation, entityId);
            }
        }

        // Initialize network sockets.
        NetworkServer.Instance.Init();
        Console.WriteLine("Network started. Port: " + Globals.Config.Port);

        // Register all [PacketHandler] methods before accepting connections.
        PacketDispatcher.Register(AuthHandler.Instance);
        PacketDispatcher.Register(AccountHandler.Instance);
        PacketDispatcher.Register(PlayerHandler.Instance);
        PacketDispatcher.Register(ChatHandler.Instance);
        PacketDispatcher.Register(PartyHandler.Instance);
        PacketDispatcher.Register(TradeHandler.Instance);
        PacketDispatcher.Register(ShopHandler.Instance);
        PacketDispatcher.Register(EditorHandler.Instance);
        Console.WriteLine($"PacketDispatcher: {PacketDispatcher.Count} handlers registered.");

        Console.WriteLine("\r\n" + "Server started. Type 'help' to see the commands." + "\r\n");

        // Start command loop on background thread.
        var dispatcher = new CommandDispatcher()
            .Register<CpsCommand>()
            .Register<DefineAccessCommand>()
            .Register<SeedCommand>();
        var consoleThread = new Thread(() => Loop.Instance.Commands(dispatcher.Dispatch, cts.Token)) { IsBackground = true };
        consoleThread.Start();

        // Start main loop and wait for cancellation.
        try
        {
            await Loop.Instance.MainAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
        }

        PerformShutdown();
    }

    private static void PerformShutdown()
    {
        // Save character data for all connected players.
        foreach (var t in WorldHost.Current.Sessions.Where(t => t.IsPlaying))
            CharacterRepository.Instance.Write(t);

        // Stop network device.
        NetworkServer.Instance.Device.Stop();
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
