using CryBits.Server.Entities;
using CryBits.Simulation.Events;

namespace CryBits.Server.Simulation.Events;

internal sealed record PlayerDisconnectedEvent : SimEvent
{
    public Player Player { get; init; }
}
