using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Shops;
using CryBits.Network.Packets.Server;
using CryBits.Server.Simulation.State;
using CryBits.Server.World;

namespace CryBits.Server.Network.Senders;

internal sealed class ShopSender(PackageSender packageSender, DefinitionCatalog catalog)
{
    private readonly DefinitionCatalog _catalog = catalog;
    public static ShopSender Instance { get; } = new(PackageSender.Instance, DefinitionCatalog.Instance);

    public void Shops(GameSession session)
    {
        packageSender.ToPlayer(session, new ShopsPacket { List = _catalog.Shops });
    }

    public void ShopOpen(EntityId entityId, Shop shop)
    {
        packageSender.ToPlayer(entityId, new ShopOpenPacket { Id = shop.GetId() });
    }
}
