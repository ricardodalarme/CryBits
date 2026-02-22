using CryBits.Entities;
using CryBits.Packets.Server;
using CryBits.Server.World;

namespace CryBits.Server.Network.Senders;

internal static class ItemSender
{
    public static void Items(GameSession session)
    {
        Send.ToPlayer(session, new ItemsPacket { List = Item.List });
    }
}
