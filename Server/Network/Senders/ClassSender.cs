using CryBits.Entities;
using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Server.Entities;
using LiteNetLib.Utils;

namespace CryBits.Server.Network.Senders;

internal static class ClassSender
{
    public static void Classes(Account account)
    {
        var data = new NetDataWriter();

        data.Put((byte)ServerPacket.Classes);
        data.WriteObject(Class.List);
        Send.ToPlayer(account, data);
    }
}