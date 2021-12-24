using System;
using System.Collections.Generic;
using System.Linq;
using CryBits.Extensions;

namespace CryBits.Entities.Shop;

[Serializable]
public class Shop : Entity
{
    // Lista de dados
    public static Dictionary<Guid, Shop> List = new();

    // Dados
    private Guid _currency;
    public Item Currency
    {
        get => Item.List.Get(_currency);
        set => _currency = value.GetId();
    }
    public IList<ShopItem> Bought { get; set; } = new List<ShopItem>();
    public IList<ShopItem> Sold { get; set; } = new List<ShopItem>();
        
    public Shop()
    {
        Name = "New shop";
        Currency = Item.List.ElementAt(0).Value;
    }

    public ShopItem FindBought(Item item) => Bought.First(x => x.Item == item);
}