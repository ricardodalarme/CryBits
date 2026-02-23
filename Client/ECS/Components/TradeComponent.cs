using CryBits.Entities.Slots;

namespace CryBits.Client.ECS.Components;

/// <summary>
/// Active trade state for the local player.
/// Added when a trade is opened, removed when it is closed or cancelled.
/// Only present on the entity with <see cref="LocalPlayerTag"/>.
/// </summary>
public sealed class TradeComponent : IComponent
{
    public ItemSlot?[]? Offer { get; set; }
    public ItemSlot?[]? TheirOffer { get; set; }
}
