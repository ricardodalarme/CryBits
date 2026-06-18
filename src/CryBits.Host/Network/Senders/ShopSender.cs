using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Shops;
using CryBits.Protocol.Packets.Server;
using CryBits.Host.Core;
using CryBits.Simulation.State;

namespace CryBits.Host.Network.Senders;

internal sealed class ShopSender(PackageSender packageSender, DefinitionCatalog catalog)
{
    public void Shops(Session session)
    {
        packageSender.ToPlayer(session, new ShopsPacket { List = catalog.Shops });
    }

    public void ShopOpen(EntityId entityId, Shop shop)
    {
        packageSender.ToPlayer(entityId, new ShopOpenPacket { Id = shop.GetId() });
    }
}
