using CryBits.Entities;
using CryBits.Packets.Server;

namespace CryBits.Client.Network.Handlers;

internal static class ItemHandler
{
    [PacketHandler]
    internal static void Items(ItemsPacket packet)
    {
        // Read items dictionary
        Item.List = packet.List;
    }
}
