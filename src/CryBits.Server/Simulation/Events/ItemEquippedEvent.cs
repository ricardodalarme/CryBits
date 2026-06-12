using CryBits.Definitions.Items;
using CryBits.Server.Entities;
using CryBits.Simulation.Events;

namespace CryBits.Server.Simulation.Events;

internal sealed record ItemEquippedEvent : SimEvent
{
    public Player Player { get; init; }
    public int EquipSlot { get; init; }
    public Item? Item { get; init; }
    public Item? OldItem { get; init; }
}
