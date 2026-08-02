namespace CryBits.Simulation.Events;

public sealed record LootDroppedEvent(
    long TickNumber,
    Guid MapId,
    int X,
    int Y,
    Guid ItemId,
    short Amount,
    long DespawnTick) : SimEvent(TickNumber);
