using CryBits.Enums;
using CryBits.Server.Entities;
using LiteNetLib.Utils;

namespace CryBits.Server.Network.Senders;

internal static class AuthSender
{
    public static void Latency(Account account)
    {
        var data = new NetDataWriter();
        data.Put((byte)ServerPacket.Latency);
        Send.ToPlayer(account, data);
    }

    public static void Connect(Account account)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.Connect);
        Send.ToPlayer(account, data);
    }
}