using CryBits.Entities.Shop;

namespace CryBits.Server.ECS.Components;

/// <summary>Active shop session for a player. Present only while a shop is open.</summary>
internal sealed class ShopComponent : ECS.IComponent
{
    public Shop Active;

    public ShopComponent(Shop shop) => Active = shop;
}
