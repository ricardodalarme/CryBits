using CryBits.Host.Core;
using CryBits.Simulation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CryBits.Host.Scheduling;

internal sealed class TickDriver
{
    public static TickDriver Instance { get; } = new();

    public async Task MainAsync(CancellationToken ct)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(1000 / SimulationConstants.TicksPerSecond));
        while (await timer.WaitForNextTickAsync(ct))
            try
            {
                WorldHost.Current.Tick();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Main loop threw an exception: {ex}");
            }
    }
}
