using CryBits.Simulation.State;
using MemoryPack;

namespace CryBits.Simulation.Intents;

[MemoryPackable]
public sealed partial record ShopBuyIntent(EntityId SourceEntityId, short Slot) : Intent(SourceEntityId);
[MemoryPackable]
public sealed partial record ShopSellIntent(EntityId SourceEntityId, byte InventorySlot, short Amount) : Intent(SourceEntityId);
[MemoryPackable]
public sealed partial record ShopCloseIntent(EntityId SourceEntityId) : Intent(SourceEntityId);
