using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Shops;
using CryBits.Protocol.Packets.Server;
using CryBits.Simulation.State;

namespace CryBits.Host.Network.Senders;

internal sealed class ShopSender(PackageSender packageSender)
{
    public void ShopOpen(EntityId entityId, Shop shop)
    {
        packageSender.ToPlayer(entityId, new ShopOpenPacket { Id = shop.Id });
    }
}
