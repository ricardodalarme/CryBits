using CryBits.Entities.Slots;

namespace CryBits.Server.ECS.Components;

/// <summary>Active trade state for a player.</summary>
internal sealed class TradeComponent : ECS.IComponent
{
    /// <summary>Entity ID of the other player in the trade; null when no trade is active.</summary>
    public int? PartnerId;

    /// <summary>Pending trade invitation from this player name.</summary>
    public string? PendingRequest;

    /// <summary>The items this player has offered in the active trade.</summary>
    public TradeSlot[]? Offer;

    /// <summary>Whether this player has accepted the current trade offer.</summary>
    public bool IsAccepted;
}
