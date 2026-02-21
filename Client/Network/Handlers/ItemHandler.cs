using CryBits.Entities;
using CryBits.Packets.Server;

namespace CryBits.Client.Network.Handlers;

internal static class ItemHandler
{
    internal static void Items(ItemsPacket packet)
    {
        // Read items dictionary
        Item.List = packet.List;
    }
}
