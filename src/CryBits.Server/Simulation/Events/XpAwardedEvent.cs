using CryBits.Server.Entities;
using CryBits.Simulation.Events;

namespace CryBits.Server.Simulation.Events;

internal sealed record XpAwardedEvent : SimEvent
{
    public Player Player { get; init; }
    public int Amount { get; init; }
}
