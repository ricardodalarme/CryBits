using CryBits.Definitions.Slots;
using CryBits.Simulation.State;
using MemoryPack;

namespace CryBits.Simulation.Components;

[MemoryPackable]
public sealed partial record TradeState(EntityId? Partner = null, EntityId? PendingInviterId = null, TradeSlot[]? Offer = null, TradeSlot[]? TheirOffer = null);
