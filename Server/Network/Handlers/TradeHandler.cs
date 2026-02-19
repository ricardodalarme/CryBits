using CryBits.Enums;
using CryBits.Server.Entities;
using CryBits.Server.Systems;
using LiteNetLib.Utils;

namespace CryBits.Server.Network.Handlers;

internal static class TradeHandler
{
    internal static void TradeInvite(Player player, NetDataReader data)
    {
        TradeSystem.Invite(player, data.GetString());
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

    internal static void TradeOffer(Player player, NetDataReader data)
    {
        TradeSystem.Offer(player, data.GetShort(), data.GetShort(), data.GetShort());
    }

    internal static void TradeOfferState(Player player, NetDataReader data)
    {
        TradeSystem.OfferState(player, (TradeStatus)data.GetByte());
    }
}
