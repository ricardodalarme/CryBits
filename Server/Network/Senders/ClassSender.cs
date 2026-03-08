using CryBits.Entities;
using CryBits.Packets.Server;
using CryBits.Server.World;

namespace CryBits.Server.Network.Senders;

internal sealed class ClassSender(PackageSender packageSender)
{
    public static ClassSender Instance { get; } = new(PackageSender.Instance);

    private readonly PackageSender _packageSender = packageSender;

    public void Classes(GameSession session)
    {
        _packageSender.ToPlayer(session, new ClassesPacket { List = Class.List });
    }
}
