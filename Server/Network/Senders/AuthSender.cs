using CryBits.Enums;
using CryBits.Server.Entities;
using LiteNetLib.Utils;

namespace CryBits.Server.Network.Senders;

internal static class AuthSender
{
    public static void Alert(Account account, string message, bool disconnect = true)
    {
        var data = new NetDataWriter();

        data.Put((byte)ServerPacket.Alert);
        data.Put(message);
        Send.ToPlayer(account, data);

        // Disconnect the account.
        if (disconnect) account.Connection.Disconnect();
    }

    public static void Latency(Account account)
    {
        var data = new NetDataWriter();
        data.Put((byte)ServerPacket.Latency);
        Send.ToPlayer(account, data);
    }

    public static void Connect(Account account)
    {
        var data = new NetDataWriter();

        data.Put((byte)ServerPacket.Connect);
        Send.ToPlayer(account, data);
    }
}