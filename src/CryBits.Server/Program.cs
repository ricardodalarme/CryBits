using CryBits.Definitions;
using CryBits.Host.Core;
using CryBits.Host.Network;
using CryBits.Host.Persistence;
using CryBits.Host.Persistence.Repositories;
using CryBits.Host.Scheduling;
using CryBits.Server.Commands;
using CryBits.Transport.Udp;
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

        // Create UDP transport and start listening.
        Console.WriteLine("Creating world.");
        var transport = new UdpTransport();
        transport.Start(Globals.Config.Port);
        var host = new WorldHost(transport);
        host.Initialize();
        Console.WriteLine("Network started. Port: " + Globals.Config.Port);

        // Register all [PacketHandler] methods before accepting connections.
        host.RegisterDefaultServices(true);
        Console.WriteLine($"PacketDispatcher: {PacketDispatcher.Count} services registered.");

        Console.WriteLine("\r\n" + "Server started. Type 'help' to see the commands." + "\r\n");

        // Start command loop on background thread.
        var dispatcher = new CommandDispatcher()
            .Register<DefineAccessCommand>()
            .Register<SeedCommand>();

        // Start command loop on background thread.
        var consoleThread = new Thread(() => ConsoleLoop.Run(dispatcher, cts.Token)) { IsBackground = true };
        consoleThread.Start();

        // Start main loop and wait for cancellation.
        try
        {
            await TickDriver.Instance.MainAsync(cts.Token);
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
        {
            var entityId = t.Character!.Value;
            var entity = WorldHost.Current.Entities.Get(entityId);
            if (entity == null) continue;
            var pos = entity.Get<CryBits.Simulation.Components.Position>();
            var appearance = entity.Get<CryBits.Simulation.Components.PlayerAppearance>();
            var stats = entity.Get<CryBits.Simulation.Components.StatBlock>();
            var vitals = entity.Get<CryBits.Simulation.Components.Vitals>();
            var inv = entity.Get<Simulation.Components.InventoryState>();
            var equip = entity.Get<CryBits.Simulation.Components.EquipmentState>();
            var hotbar = entity.Get<CryBits.Simulation.Components.HotbarState>();
            if (pos == null || appearance == null || stats == null || vitals == null ||
                inv == null || equip == null || hotbar == null) continue;

            var data = new CryBits.Definitions.Characters.Character
            {
                Name = appearance.Name,
                ClassId = appearance.ClassId,
                Gender = appearance.Gender,
                TextureNum = appearance.TextureNum,
                Level = stats.Level,
                Experience = stats.Experience,
                Points = stats.Points,
                Attributes = (short[])stats.Attribute.Clone(),
                MapId = pos.MapId,
                X = pos.X,
                Y = pos.Y,
                Direction = (byte)pos.Direction,
                Hp = vitals.Hp,
                Mp = vitals.Mp,
                InventoryIds = new Guid[CryBits.Definitions.Globals.MaxInventory],
                InventoryAmounts = new short[CryBits.Definitions.Globals.MaxInventory],
                Equipment = new Guid[(byte)CryBits.Definitions.Items.Equipment.Count],
                HotbarTypes = new byte[CryBits.Definitions.Globals.MaxHotbar],
                HotbarSlots = new byte[CryBits.Definitions.Globals.MaxHotbar],
            };
            for (byte i = 0; i < CryBits.Definitions.Globals.MaxInventory; i++)
            {
                data.InventoryIds[i] = inv.Slots[i].ItemId;
                data.InventoryAmounts[i] = inv.Slots[i].Amount;
            }
            for (byte i = 0; i < (byte)CryBits.Definitions.Items.Equipment.Count; i++)
                data.Equipment[i] = equip.Slots[i];
            for (byte i = 0; i < CryBits.Definitions.Globals.MaxHotbar; i++)
            {
                data.HotbarTypes[i] = (byte)hotbar.Slots[i].Type;
                data.HotbarSlots[i] = (byte)hotbar.Slots[i].Slot;
            }

            CharacterRepository.Instance.Write(t.Account!, data);
        }

        // Stop network device.
        WorldHost.Current.Transport.Stop();
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
