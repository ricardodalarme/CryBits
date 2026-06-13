using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Shops;
using CryBits.Network.Packets.Server;
using CryBits.Host.Core;
using CryBits.Simulation.State;

namespace CryBits.Host.Network.Senders;

internal sealed class ShopSender(PackageSender packageSender, DefinitionCatalog catalog)
{
    private readonly DefinitionCatalog _catalog = catalog;
    public static ShopSender Instance { get; } = new(PackageSender.Instance, DefinitionCatalog.Instance);

    public void Shops(Session session)
    {
        packageSender.ToPlayer(session, new ShopsPacket { List = _catalog.Shops });
    }

    public void ShopOpen(EntityId entityId, Shop shop)
    {
        packageSender.ToPlayer(entityId, new ShopOpenPacket { Id = shop.GetId() });
    }
}
