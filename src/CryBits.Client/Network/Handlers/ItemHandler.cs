using CryBits.Entities;
using CryBits.Packets.Server;

namespace CryBits.Client.Network.Handlers;

internal class ItemHandler
{
    [PacketHandler]
    internal void Items(ItemsPacket packet)
    {
        // Read items dictionary
        Item.List = packet.List;
    }
}
