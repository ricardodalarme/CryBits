using CryBits.Entities;
using CryBits.Packets.Server;
using CryBits.Server.Entities;

namespace CryBits.Server.Network.Senders;

internal static class ClassSender
{
    public static void Classes(Account account)
    {
        Send.ToPlayer(account, new ClassesPacket { List = Class.List });
    }
}
