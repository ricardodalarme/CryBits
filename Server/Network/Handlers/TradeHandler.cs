using CryBits.Enums;
using CryBits.Packets.Client;
using CryBits.Server.Entities;
using CryBits.Server.Systems;

namespace CryBits.Server.Network.Handlers;

internal sealed class TradeHandler(TradeSystem tradeSystem)
{
    public static TradeHandler Instance { get; } = new(TradeSystem.Instance);

    [PacketHandler]
    internal void TradeInvite(Player player, TradeInvitePacket packet)
    {
        tradeSystem.Invite(player, packet.PlayerName);
    }

    [PacketHandler]
    internal void TradeAccept(Player player, TradeAcceptPacket _)
    {
        tradeSystem.Accept(player);
    }

    [PacketHandler]
    internal void TradeDecline(Player player, TradeDeclinePacket _)
    {
        tradeSystem.Decline(player);
    }

    [PacketHandler]
    internal void TradeLeave(Player player, TradeLeavePacket _)
    {
        tradeSystem.Leave(player);
    }

    [PacketHandler]
    internal void TradeOffer(Player player, TradeOfferPacket packet)
    {
        tradeSystem.Offer(player, packet.Slot, packet.InventorySlot, packet.Amount);
    }

    [PacketHandler]
    internal void TradeOfferState(Player player, TradeOfferStatePacket packet)
    {
        tradeSystem.OfferState(player, (TradeStatus)packet.State);
    }
}
