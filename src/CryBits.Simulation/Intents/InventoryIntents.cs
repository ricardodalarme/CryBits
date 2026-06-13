using CryBits.Simulation.State;

namespace CryBits.Simulation.Intents;

public sealed record CollectItemIntent(EntityId SourceEntityId) : Intent(SourceEntityId);
public sealed record DropItemIntent(EntityId SourceEntityId, int SlotIndex, short Amount) : Intent(SourceEntityId);
public sealed record InventoryUseIntent(EntityId SourceEntityId, int SlotIndex) : Intent(SourceEntityId);
public sealed record InventorySwapIntent(EntityId SourceEntityId, short SlotOld, short SlotNew) : Intent(SourceEntityId);
public sealed record EquipmentRemoveIntent(EntityId SourceEntityId, byte Slot) : Intent(SourceEntityId);
