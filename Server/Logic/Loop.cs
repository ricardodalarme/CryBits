using CryBits.Server.Commands;
using CryBits.Server.Network;
using CryBits.Server.Systems;
using CryBits.Server.World;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CryBits.Server.Logic;

internal sealed class Loop(
    NetworkServer networkServer,
    MapItemSystem mapItemSystem,
    RegenerationSystem regenerationSystem)
{
    public static Loop Instance { get; } = new(
        NetworkServer.Instance,
        MapItemSystem.Instance,
        RegenerationSystem.Instance);

    // Target simulation rate: 20 ticks per second (50ms per tick)
    private const int TicksPerSecond = 20;

    // Measured loops per second (static so CpsCommand can access without Instance).
    public static int Cps;

    // Timing counters (static so RegenerationSystem/MapItemSystem can access without Instance).
    private long _timer500, _timer1000;
    public static long TimerRegeneration;
    public static long TimerMapItems;

    public async Task MainAsync(CancellationToken ct)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(1000 / TicksPerSecond));
        var cps = 0;

        while (await timer.WaitForNextTickAsync(ct))
        {
            try
            {
                // Handle incoming network data.
                networkServer.HandleData();

                var now = Environment.TickCount64;

                if (now > _timer500 + 500)
                {
                    // Map logic
                    foreach (var tempMap in GameWorld.Current.Maps.Values)
                    {
                        mapItemSystem.Tick(tempMap);
                        tempMap.Logic();
                    }

                    // Player vital regeneration
                    foreach (var session in GameWorld.Current.Sessions.Where(a => a.IsPlaying))
                        regenerationSystem.Tick(session.Character!);

                    // Reset 500 ms timer.
                    _timer500 = now;
                }

                // Reset longer-running timers.
                if (now > TimerRegeneration + 5000) TimerRegeneration = now;
                if (now > TimerMapItems + 300000) TimerMapItems = now;

                // Compute CPS.
                if (_timer1000 < now)
                {
                    Cps = cps;
                    cps = 0;
                    _timer1000 = now + 1000;
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
