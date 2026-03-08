using CryBits.Entities.Shop;
using CryBits.Extensions;
using CryBits.Packets.Server;
using CryBits.Server.Entities;
using CryBits.Server.World;

namespace CryBits.Server.Network.Senders;

internal sealed class ShopSender(PackageSender packageSender)
{
    public static ShopSender Instance { get; } = new(PackageSender.Instance);

    private readonly PackageSender _packageSender = packageSender;

    public void Shops(GameSession session)
    {
        _packageSender.ToPlayer(session, new ShopsPacket { List = Shop.List });
    }

    public void ShopOpen(Player player, Shop shop)
    {
        _packageSender.ToPlayer(player, new ShopOpenPacket { Id = shop.GetId() });
    }
}
