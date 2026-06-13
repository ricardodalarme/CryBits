using CryBits.Definitions.Common;
using CryBits.Simulation.State;

namespace CryBits.Simulation.Intents;

public sealed record TradeInviteIntent(EntityId SourceEntityId, string PlayerName) : Intent(SourceEntityId);
public sealed record TradeAcceptIntent(EntityId SourceEntityId) : Intent(SourceEntityId);
public sealed record TradeDeclineIntent(EntityId SourceEntityId) : Intent(SourceEntityId);
public sealed record TradeLeaveIntent(EntityId SourceEntityId) : Intent(SourceEntityId);
public sealed record TradeOfferIntent(EntityId SourceEntityId, short OfferSlot, short InventorySlot, short Amount) : Intent(SourceEntityId);
public sealed record TradeOfferStateIntent(EntityId SourceEntityId, TradeStatus State) : Intent(SourceEntityId);
