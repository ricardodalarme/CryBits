using CryBits.Definitions.Slots;
using CryBits.Simulation.State;

namespace CryBits.Simulation.Components;

public sealed class TradeState
{
    public EntityId? Partner { get; set; }
    public EntityId? PendingInviterId { get; set; }
    public TradeSlot[]? Offer { get; set; }
    public TradeSlot[]? TheirOffer { get; set; }
}
