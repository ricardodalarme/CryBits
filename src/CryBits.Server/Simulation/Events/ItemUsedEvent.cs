using CryBits.Definitions.Items;
using CryBits.Server.Entities;
using CryBits.Simulation.Events;

namespace CryBits.Server.Simulation.Events;

internal sealed record ItemUsedEvent : SimEvent
{
    public Player Player { get; init; }
    public int SlotIndex { get; init; }
    public Item Item { get; init; }
}
