using CryBits.Entities;
using CryBits.Packets.Server;
using CryBits.Server.World;

namespace CryBits.Server.Network.Senders;

internal sealed class ClassSender(PackageSender packageSender)
{
    public static ClassSender Instance { get; } = new(PackageSender.Instance);

    public void Classes(GameSession session)
    {
        packageSender.ToPlayer(session, new ClassesPacket { List = Class.List });
    }
}
