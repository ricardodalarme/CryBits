using CryBits.Host.Core;
using CryBits.Simulation;

namespace CryBits.Host.Scheduling;

internal sealed class TickDriver(WorldHost host)
{
    public async Task MainAsync(CancellationToken ct)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(1000 / SimulationConstants.TicksPerSecond));
        while (await timer.WaitForNextTickAsync(ct))
            try
            {
                host.Tick();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Main loop threw an exception: {ex}");
            }
    }
}
