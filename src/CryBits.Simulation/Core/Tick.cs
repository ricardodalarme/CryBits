using CryBits.Simulation.Events;
using CryBits.Simulation.Intents;

namespace CryBits.Simulation.Core;

public sealed record Tick(
    long TickNumber,
    IntentBuffer Intents,
    EventBuffer Events);
