using CryBits.Definitions.Items;
using CryBits.Definitions.Slots;
using System;

namespace CryBits.Definitions.Shops;

[Serializable]
public class ShopItem(Item item, short amount, short price) : ItemSlot(item, amount)
{
    public short Price { get; set; } = price;

    public override string ToString() => Item.Name + " - " + Amount + "x [$" + Price + "]";
}
