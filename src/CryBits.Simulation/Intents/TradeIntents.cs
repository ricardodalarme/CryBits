using CryBits.Definitions.Common;
using CryBits.Simulation.State;
using MemoryPack;

namespace CryBits.Simulation.Intents;

[MemoryPackable]
public sealed partial record TradeInviteIntent(EntityId SourceEntityId, string PlayerName) : Intent(SourceEntityId);
[MemoryPackable]
public sealed partial record TradeAcceptIntent(EntityId SourceEntityId) : Intent(SourceEntityId);
[MemoryPackable]
public sealed partial record TradeDeclineIntent(EntityId SourceEntityId) : Intent(SourceEntityId);
[MemoryPackable]
public sealed partial record TradeLeaveIntent(EntityId SourceEntityId) : Intent(SourceEntityId);
[MemoryPackable]
public sealed partial record TradeOfferIntent(EntityId SourceEntityId, short OfferSlot, short InventorySlot, short Amount) : Intent(SourceEntityId);
[MemoryPackable]
public sealed partial record TradeOfferStateIntent(EntityId SourceEntityId, TradeStatus State) : Intent(SourceEntityId);
