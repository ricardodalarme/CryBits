using CryBits.Simulation.Core;
using MemoryPack;

namespace CryBits.Simulation.Intents;

[MemoryPackable]
public sealed partial record CollectItemIntent(EntityId SourceEntityId) : Intent(SourceEntityId);

[MemoryPackable]
public sealed partial record DropItemIntent(EntityId SourceEntityId, byte SlotIndex, short Amount)
    : Intent(SourceEntityId);

[MemoryPackable]
public sealed partial record InventoryUseIntent(EntityId SourceEntityId, int SlotIndex) : Intent(SourceEntityId);

[MemoryPackable]
public sealed partial record InventorySwapIntent(EntityId SourceEntityId, short SlotOld, short SlotNew)
    : Intent(SourceEntityId);

[MemoryPackable]
public sealed partial record EquipmentRemoveIntent(EntityId SourceEntityId, byte Slot) : Intent(SourceEntityId);
