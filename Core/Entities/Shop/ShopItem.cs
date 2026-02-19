using System;
using CryBits.Entities.Slots;

namespace CryBits.Entities.Shop;

[Serializable]
public class ShopItem(Item item, short amount, short price) : ItemSlot(item, amount)
{
    public short Price { get; set; } = price;

    public override string ToString() => Item.Name + " - " + Amount + "x [$" + Price + "]";
}
