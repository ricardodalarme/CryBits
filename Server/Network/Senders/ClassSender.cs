using CryBits.Entities;
using CryBits.Packets.Server;
using CryBits.Server.World;

namespace CryBits.Server.Network.Senders;

internal static class ClassSender
{
    public static void Classes(GameSession session)
    {
        Send.ToPlayer(session, new ClassesPacket { List = Class.List });
    }
}
