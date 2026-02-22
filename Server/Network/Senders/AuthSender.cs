using CryBits.Packets.Server;
using CryBits.Server.World;

namespace CryBits.Server.Network.Senders;

internal static class AuthSender
{
    public static void Alert(GameSession session, string message, bool disconnect = true)
    {
        Send.ToPlayer(session, new AlertPacket { Message = message });

        if (disconnect) session.Connection.Disconnect();
    }

    public static void Latency(GameSession session)
    {
        Send.ToPlayer(session, new LatencyPacket());
    }

    public static void Connect(GameSession session)
    {
        Send.ToPlayer(session, new ConnectPacket());
    }
}
