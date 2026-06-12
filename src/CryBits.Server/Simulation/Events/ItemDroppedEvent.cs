using CryBits.Server.Entities;
using CryBits.Simulation.Events;

namespace CryBits.Server.Simulation.Events;

internal sealed record ItemDroppedEvent : SimEvent
{
    public Player Player { get; init; }
    public int SlotIndex { get; init; }
    public short Amount { get; init; }
}
