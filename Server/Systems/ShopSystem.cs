using CryBits.Entities.Shop;
using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;

namespace CryBits.Server.Systems;

/// <summary>
/// System that owns all shop lifecycle logic.
/// </summary>
internal static class ShopSystem
{
    /// <summary>
    /// Opens <paramref name="shop"/> for <paramref name="player"/> and notifies the client.
    /// </summary>
    public static void Open(Player player, Shop shop)
    {
        player.Shop = shop;
        ShopSender.ShopOpen(player, shop);
    }

    /// <summary>
    /// Closes the active shop session for <paramref name="player"/> and notifies the client.
    /// No-ops if the player has no open shop.
    /// </summary>
    public static void Leave(Player player)
    {
        if (player.Shop == null) return;

        player.Shop = null;
        ShopSender.ShopOpen(player, null);
    }
}
