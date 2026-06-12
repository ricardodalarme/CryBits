using CryBits.Simulation.Events;

namespace CryBits.Server.Simulation.Core;

internal sealed record Tick(
    long TickNumber,
    EventBuffer Events);
