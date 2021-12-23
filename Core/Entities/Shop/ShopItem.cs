using System;
using CryBits.Entities.Slots;

namespace CryBits.Entities.Shop;

[Serializable]
public class ShopItem : ItemSlot
{
    public short Price { get; set; }

    public ShopItem(Item item, short amount, short price) : base(item, amount)
    {
        Price = price;
    }

    public override string ToString() => Item.Name + " - " + Amount + "x [$" + Price + "]";
}