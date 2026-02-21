using CryBits.Enums;
using CryBits.Packets.Client;
using CryBits.Server.Entities;
using CryBits.Server.Systems;

namespace CryBits.Server.Network.Handlers;

internal static class TradeHandler
{
    [PacketHandler(ClientPacket.TradeInvite)]
    internal static void TradeInvite(Player player, TradeInvitePacket packet)
    {
        TradeSystem.Invite(player, packet.PlayerName);
    }

    [PacketHandler(ClientPacket.TradeAccept)]
    internal static void TradeAccept(Player player)
    {
        TradeSystem.Accept(player);
    }

    [PacketHandler(ClientPacket.TradeDecline)]
    internal static void TradeDecline(Player player)
    {
        TradeSystem.Decline(player);
    }

    [PacketHandler(ClientPacket.TradeLeave)]
    internal static void TradeLeave(Player player)
    {
        TradeSystem.Leave(player);
    }

    [PacketHandler(ClientPacket.TradeOffer)]
    internal static void TradeOffer(Player player, TradeOfferPacket packet)
    {
        TradeSystem.Offer(player, packet.Slot, packet.InventorySlot, packet.Amount);
    }

    [PacketHandler(ClientPacket.TradeOfferState)]
    internal static void TradeOfferState(Player player, TradeOfferStatePacket packet)
    {
        TradeSystem.OfferState(player, (TradeStatus)packet.State);
    }
}
