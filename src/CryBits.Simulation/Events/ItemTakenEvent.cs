using CryBits.Simulation.State;

namespace CryBits.Simulation.Events;

public sealed record ItemTakenEvent : SimEvent
{
    public EntityId EntityId { get; init; }
    public byte SlotIndex { get; init; }
    public short Amount { get; init; }
}
