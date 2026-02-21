using CryBits.Enums;
using CryBits.Packets.Client;
using CryBits.Server.Entities;
using CryBits.Server.Systems;

namespace CryBits.Server.Network.Handlers;

internal static class TradeHandler
{
    internal static void TradeInvite(Player player, TradeInvitePacket packet)
    {
        TradeSystem.Invite(player, packet.PlayerName);
    }

    internal static void TradeAccept(Player player)
    {
        TradeSystem.Accept(player);
    }

    internal static void TradeDecline(Player player)
    {
        TradeSystem.Decline(player);
    }

    internal static void TradeLeave(Player player)
    {
        TradeSystem.Leave(player);
    }

    internal static void TradeOffer(Player player, TradeOfferPacket packet)
    {
        TradeSystem.Offer(player, packet.Slot, packet.InventorySlot, packet.Amount);
    }

    internal static void TradeOfferState(Player player, TradeOfferStatePacket packet)
    {
        TradeSystem.OfferState(player, (TradeStatus)packet.State);
    }
}
