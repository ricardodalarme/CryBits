namespace CryBits.Simulation.Events;

public sealed partial record class LootDroppedEvent(long TickNumber, Guid MapId, byte X, byte Y, Guid ItemId, short Amount, long DespawnTick) : SimEvent(TickNumber);
