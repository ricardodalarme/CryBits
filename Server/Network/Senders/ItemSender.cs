using CryBits.Entities;
using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Server.Entities;
using LiteNetLib.Utils;

namespace CryBits.Server.Network.Senders;

internal static class ItemSender
{
    public static void Items(Account account)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.Items);
        data.WriteObject(Item.List);
        Send.ToPlayer(account, data);
    }
}
