using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Packets.Server;
using CryBits.Server.Entities;
using static CryBits.Globals;

namespace CryBits.Server.Network.Senders;

internal static class TradeSender
{
    public static void Trade(Player player, bool state)
    {
        Send.ToPlayer(player, new TradePacket { State = state });
    }

    public static void TradeInvitation(Player player, string playerInvitation)
    {
        Send.ToPlayer(player, new TradeInvitationPacket { PlayerInvitation = playerInvitation });
    }

    public static void TradeState(Player player, TradeStatus state)
    {
        Send.ToPlayer(player, new TradeStatePacket { State = (byte)state });
    }

    public static void TradeOffer(Player player, bool own = true)
    {
        var to = own ? player : player.Trade;
        var packet = new TradeOfferPacket
        {
            Own = own,
            Items = new PacketsTradeOfferItem[MaxInventory]
        };
        for (short i = 0; i < MaxInventory; i++)
        {
            packet.Items[i] = new PacketsTradeOfferItem
            {
                ItemId = to.Inventory[to.TradeOffer[i].SlotNum].Item.GetId(),
                Amount = to.TradeOffer[i].Amount
            };
        }
        Send.ToPlayer(player, packet);
    }
}
