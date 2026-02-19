using CryBits.Enums;
using LiteNetLib.Utils;

namespace CryBits.Client.Network.Senders;

internal static class TradeSender
{
    public static void TradeInvite(string playerName)
    {
        var data = new NetDataWriter();

        data.Put((byte)ClientPacket.TradeInvite);
        data.Put(playerName);
        Send.Packet(data);
    }

    public static void TradeAccept()
    {
        var data = new NetDataWriter();

        data.Put((byte)ClientPacket.TradeAccept);
        Send.Packet(data);
    }

    public static void TradeDecline()
    {
        var data = new NetDataWriter();

        data.Put((byte)ClientPacket.TradeDecline);
        Send.Packet(data);
    }

    public static void TradeLeave()
    {
        var data = new NetDataWriter();

        data.Put((byte)ClientPacket.TradeLeave);
        Send.Packet(data);
    }

    public static void TradeOffer(short slot, short inventorySlot, short amount = 1)
    {
        var data = new NetDataWriter();

        data.Put((byte)ClientPacket.TradeOffer);
        data.Put(slot);
        data.Put(inventorySlot);
        data.Put(amount);
        Send.Packet(data);
    }

    public static void TradeOfferState(TradeStatus state)
    {
        var data = new NetDataWriter();

        data.Put((byte)ClientPacket.TradeOfferState);
        data.Put((byte)state);
        Send.Packet(data);
    }
}
