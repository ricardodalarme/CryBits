using CryBits.Definitions.Catalog;
using CryBits.Definitions.Common;
using CryBits.Definitions.Items;
using CryBits.Definitions.Helpers.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CryBits.Definitions.Shops;

[Serializable]
public class Shop : Entity
{
    private Guid _currency;

    public Item Currency
    {
        get => DefinitionCatalog.Instance.Items.Get(_currency);
        set => _currency = value.GetId();
    }

    public IList<ShopItem> Bought { get; set; } = [];
    public IList<ShopItem> Sold { get; set; } = [];

    public Shop()
    {
        Name = "New shop";
        Currency = DefinitionCatalog.Instance.Items.ElementAt(0).Value;
    }

    public ShopItem FindBought(Item item) => Bought.First(x => x.Item == item);
}
