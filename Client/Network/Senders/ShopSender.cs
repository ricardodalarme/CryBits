using CryBits.Enums;
using LiteNetLib.Utils;

namespace CryBits.Client.Network.Senders;

internal static class ShopSender
{
    public static void ShopBuy(short slot)
    {
        var data = new NetDataWriter();

        data.Put((byte)ClientPacket.ShopBuy);
        data.Put(slot);
        Send.Packet(data);
    }

    public static void ShopSell(short slot, short amount)
    {
        var data = new NetDataWriter();

        data.Put((byte)ClientPacket.ShopSell);
        data.Put(slot);
        data.Put(amount);
        Send.Packet(data);
    }

    public static void ShopClose()
    {
        var data = new NetDataWriter();

        data.Put((byte)ClientPacket.ShopClose);
        Send.Packet(data);
    }
}
