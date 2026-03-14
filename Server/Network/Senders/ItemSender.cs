using CryBits.Entities;
using CryBits.Packets.Server;
using CryBits.Server.World;

namespace CryBits.Server.Network.Senders;

internal sealed class ItemSender(PackageSender packageSender)
{
    public static ItemSender Instance { get; } = new(PackageSender.Instance);

    public void Items(GameSession session)
    {
        packageSender.ToPlayer(session, new ItemsPacket { List = Item.List });
    }
}
