using CryBits.Host.Core;
using CryBits.Simulation;
using Microsoft.Extensions.Logging;
using ZLogger;

namespace CryBits.Host.Scheduling;

internal sealed class TickDriver(WorldHost host, ILogger<TickDriver> logger)
{
    public async Task MainAsync(CancellationToken ct)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(1000.0 / SimulationConstants.TicksPerSecond));
        while (await timer.WaitForNextTickAsync(ct))
            try
            {
                host.Tick();
            }
            catch (Exception ex)
            {
                logger.ZLogError(ex, $"Tick loop threw an exception");
            }
    }
}
