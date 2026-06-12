using CryBits.Server.Entities;
using CryBits.Simulation.Events;

namespace CryBits.Server.Simulation.Events;

internal sealed record EntityDiedEvent : SimEvent
{
    public Character Entity { get; init; }
    public Character? Source { get; init; }
}
