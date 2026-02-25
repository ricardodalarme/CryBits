using CryBits.Entities.Slots;

namespace CryBits.Client.Components;

/// <summary>
/// Active trade state for the local player.
/// Added when a trade is opened, removed when it is closed or canceled.
/// </summary>
internal struct TradeComponent()
{
    public ItemSlot?[] Offer = [];
    public ItemSlot?[] TheirOffer = [];
}
