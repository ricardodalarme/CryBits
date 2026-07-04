using CryBits.Simulation.State;

namespace CryBits.Simulation.Events;

public sealed record ItemEquippedEvent(long TickNumber, EntityId PlayerId, int EquipSlot, Guid? ItemId, Guid? OldItemId) : SimEvent(TickNumber);
