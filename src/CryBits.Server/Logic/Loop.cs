using CryBits.Server.Commands;
using CryBits.Server.Network;
using CryBits.Server.Simulation.Core;
using CryBits.Server.Systems;
using CryBits.Server.World;
using CryBits.Simulation.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CryBits.Server.Logic;

internal sealed class Loop(NetworkServer networkServer, TickPipeline pipeline)
{
    public static Loop Instance { get; } = new(
        NetworkServer.Instance,
        TickPipeline.CreateDefault());

    // Target simulation rate: 20 ticks per second (50ms per tick)
    private const int TicksPerSecond = 20;

    // Measured loops per second (static so CpsCommand can access without Instance).
    public static int Cps;
    private long _cpsTimer;

    public async Task MainAsync(CancellationToken ct)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(1000 / TicksPerSecond));
        var cps = 0;

        while (await timer.WaitForNextTickAsync(ct))
        {
            try
            {
                var tick = new Tick(Environment.TickCount64, new EventBuffer());
                GameWorld.Current.CurrentTick = tick;

                // Handle incoming network data — handlers call systems which emit events.
                networkServer.HandleData();

                // Run pipeline — systems react to events emitted during handler processing.
                pipeline.Execute(GameWorld.Current, tick);

                GameWorld.Current.CurrentTick = null;

                // Compute CPS.
                if (_cpsTimer < Environment.TickCount64)
                {
                    Cps = cps;
                    cps = 1;
                    _cpsTimer = Environment.TickCount64 + 1000;
                }
                else
                    cps++;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Main loop threw an exception: {ex}");
            }
        }
    }

    public void Commands(CancellationToken ct)
    {
        var dispatcher = new CommandDispatcher()
            .Register<CpsCommand>()
            .Register<DefineAccessCommand>()
            .Register<SeedCommand>();

        // Console command loop.
        while (!ct.IsCancellationRequested)
        {
            try
            {
                Console.Write("Execute: ");
                dispatcher.Dispatch(Console.ReadLine());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Command loop threw an exception: {ex}");
            }
        }
    }
}
