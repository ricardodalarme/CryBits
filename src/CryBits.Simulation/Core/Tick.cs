using CryBits.Simulation.Events;

namespace CryBits.Simulation.Core;

public sealed record Tick(
    long TickNumber,
    EventBuffer Events);
