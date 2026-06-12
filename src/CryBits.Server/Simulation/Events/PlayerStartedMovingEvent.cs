using CryBits.Server.Entities;
using CryBits.Simulation.Events;

namespace CryBits.Server.Simulation.Events;

internal sealed record PlayerStartedMovingEvent : SimEvent
{
    public Player Player { get; init; }
}
