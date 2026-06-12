using CryBits.Definitions.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CryBits.Definitions.Shops;

[Serializable]
public class Shop : Entity
{
    public Guid CurrencyId { get; set; }

    public IList<ShopItem> Bought { get; set; } = [];
    public IList<ShopItem> Sold { get; set; } = [];

    public Shop()
    {
        Name = "New shop";
    }

    public ShopItem FindBought(Guid itemId) => Bought.First(x => x.ItemId == itemId);
}
