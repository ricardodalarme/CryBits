using CryBits.Packets.Server;
using CryBits.Server.Entities;

namespace CryBits.Server.Network.Senders;

internal static class AuthSender
{
    public static void Alert(Account account, string message, bool disconnect = true)
    {
        Send.ToPlayer(account, new AlertPacket { Message = message });

        // Disconnect the account.
        if (disconnect) account.Connection.Disconnect();
    }

    public static void Latency(Account account)
    {
        Send.ToPlayer(account, new LatencyPacket());
    }

    public static void Connect(Account account)
    {
        Send.ToPlayer(account, new ConnectPacket());
    }
}
