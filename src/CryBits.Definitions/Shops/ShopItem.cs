using CryBits.Definitions.Slots;
using System;

namespace CryBits.Definitions.Shops;

[Serializable]
public class ShopItem(Guid itemId, short amount, short price) : ItemSlot(itemId, amount)
{
    public short Price { get; set; } = price;

    public override string ToString() => ItemId + " - " + Amount + "x [$" + Price + "]";
}
