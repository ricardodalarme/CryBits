using CryBits.Entities;
using CryBits.Packets.Server;
using CryBits.Server.Entities;

namespace CryBits.Server.Network.Senders;

internal static class ItemSender
{
    public static void Items(Account account)
    {
        Send.ToPlayer(account, new ItemsPacket { List = Item.List });
    }
}
