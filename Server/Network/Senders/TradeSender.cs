using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Server.Entities;
using LiteNetLib.Utils;
using static CryBits.Globals;

namespace CryBits.Server.Network.Senders;

internal static class TradeSender
{
    public static void Trade(Player player, bool state)
    {
        var data = new NetDataWriter();

        data.Put((byte)ServerPacket.Trade);
        data.Put(state);
        Send.ToPlayer(player, data);
    }

    public static void TradeInvitation(Player player, string playerInvitation)
    {
        var data = new NetDataWriter();

        data.Put((byte)ServerPacket.TradeInvitation);
        data.Put(playerInvitation);
        Send.ToPlayer(player, data);
    }

    public static void TradeState(Player player, TradeStatus state)
    {
        var data = new NetDataWriter();

        data.Put((byte)ServerPacket.TradeState);
        data.Put((byte)state);
        Send.ToPlayer(player, data);
    }

    public static void TradeOffer(Player player, bool own = true)
    {
        var data = new NetDataWriter();
        var to = own ? player : player.Trade;

        data.Put((byte)ServerPacket.TradeOffer);
        data.Put(own);
        for (byte i = 0; i < MaxInventory; i++)
        {
            data.Put(to.Inventory[to.TradeOffer[i].SlotNum].Item.GetId());
            data.Put(to.TradeOffer[i].Amount);
        }

        Send.ToPlayer(player, data);
    }
}
