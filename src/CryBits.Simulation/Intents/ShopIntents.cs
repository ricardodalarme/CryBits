using CryBits.Simulation.State;

namespace CryBits.Simulation.Intents;

public sealed record ShopBuyIntent(EntityId SourceEntityId, short Slot) : Intent(SourceEntityId);
public sealed record ShopSellIntent(EntityId SourceEntityId, byte InventorySlot, short Amount) : Intent(SourceEntityId);
public sealed record ShopCloseIntent(EntityId SourceEntityId) : Intent(SourceEntityId);
