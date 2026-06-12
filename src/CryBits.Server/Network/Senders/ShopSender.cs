using CryBits.Definitions.Catalog;
using CryBits.Definitions.Shops;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Packets.Server;
using CryBits.Server.Entities;
using CryBits.Server.World;

namespace CryBits.Server.Network.Senders;

internal sealed class ShopSender(PackageSender packageSender)
{
    public static ShopSender Instance { get; } = new(PackageSender.Instance);

    public void Shops(GameSession session)
    {
        packageSender.ToPlayer(session, new ShopsPacket { List = DefinitionCatalog.Shops });
    }

    public void ShopOpen(Player player, Shop shop)
    {
        packageSender.ToPlayer(player, new ShopOpenPacket { Id = shop.GetId() });
    }
}
