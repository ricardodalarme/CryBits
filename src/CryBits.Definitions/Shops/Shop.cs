using CryBits.Definitions.Catalog;
using CryBits.Definitions.Common;
using CryBits.Definitions.Items;
using CryBits.Definitions.Helpers.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace CryBits.Definitions.Shops;

[Serializable]
public class Shop : Entity
{
    private Guid _currency;

    [JsonIgnore]
    public Item Currency
    {
        get => DefinitionCatalog.Instance.Items.Get(_currency);
        set => _currency = value.GetId();
    }

    [JsonInclude]
    private Guid CurrencyId { get => _currency; set => _currency = value; }

    public IList<ShopItem> Bought { get; set; } = [];
    public IList<ShopItem> Sold { get; set; } = [];

    public Shop()
    {
        Name = "New shop";
        Currency = DefinitionCatalog.Instance.Items.ElementAt(0).Value;
    }

    public ShopItem FindBought(Item item) => Bought.First(x => x.Item == item);
}
