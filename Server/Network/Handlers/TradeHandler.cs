using CryBits.Enums;
using CryBits.Packets.Client;
using CryBits.Server.Entities;
using CryBits.Server.Systems;

namespace CryBits.Server.Network.Handlers;

internal static class TradeHandler
{
    [PacketHandler]
    internal static void TradeInvite(Player player, TradeInvitePacket packet)
    {
        TradeSystem.Invite(player, packet.PlayerName);
    }

    [PacketHandler]
    internal static void TradeAccept(Player player, TradeAcceptPacket _)
    {
        TradeSystem.Accept(player);
    }

    [PacketHandler]
    internal static void TradeDecline(Player player, TradeDeclinePacket _)
    {
        TradeSystem.Decline(player);
    }

    [PacketHandler]
    internal static void TradeLeave(Player player, TradeLeavePacket _)
    {
        TradeSystem.Leave(player);
    }

    [PacketHandler]
    internal static void TradeOffer(Player player, TradeOfferPacket packet)
    {
        TradeSystem.Offer(player, packet.Slot, packet.InventorySlot, packet.Amount);
    }

    [PacketHandler]
    internal static void TradeOfferState(Player player, TradeOfferStatePacket packet)
    {
        TradeSystem.OfferState(player, (TradeStatus)packet.State);
    }
}
