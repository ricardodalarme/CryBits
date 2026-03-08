using CryBits.Enums;
using CryBits.Packets.Client;
using CryBits.Server.Entities;
using CryBits.Server.Systems;

namespace CryBits.Server.Network.Handlers;

internal sealed class TradeHandler(TradeSystem tradeSystem)
{
    public static TradeHandler Instance { get; } = new(TradeSystem.Instance);

    private readonly TradeSystem _tradeSystem = tradeSystem;

    [PacketHandler]
    internal void TradeInvite(Player player, TradeInvitePacket packet)
    {
        _tradeSystem.Invite(player, packet.PlayerName);
    }

    [PacketHandler]
    internal void TradeAccept(Player player, TradeAcceptPacket _)
    {
        _tradeSystem.Accept(player);
    }

    [PacketHandler]
    internal void TradeDecline(Player player, TradeDeclinePacket _)
    {
        _tradeSystem.Decline(player);
    }

    [PacketHandler]
    internal void TradeLeave(Player player, TradeLeavePacket _)
    {
        _tradeSystem.Leave(player);
    }

    [PacketHandler]
    internal void TradeOffer(Player player, TradeOfferPacket packet)
    {
        _tradeSystem.Offer(player, packet.Slot, packet.InventorySlot, packet.Amount);
    }

    [PacketHandler]
    internal void TradeOfferState(Player player, TradeOfferStatePacket packet)
    {
        _tradeSystem.OfferState(player, (TradeStatus)packet.State);
    }
}
