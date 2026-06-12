using CryBits.Server.Entities;
using CryBits.Simulation.Events;

namespace CryBits.Server.Simulation.Events;

internal sealed record DamageDealtEvent : SimEvent
{
    public Character Source { get; init; }
    public Character Target { get; init; }
    public short Damage { get; init; }
}
