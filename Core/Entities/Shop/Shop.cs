using System;
using System.Collections.Generic;
using System.Linq;
using CryBits.Extensions;

namespace CryBits.Entities.Shop;

[Serializable]
public class Shop : Entity
{
    // Lista de dados
    public static Dictionary<Guid, Shop> List = [];

    // Dados
    private Guid _currency;
    public Item Currency
    {
        get => Item.List.Get(_currency);
        set => _currency = value.GetId();
    }
    public IList<ShopItem> Bought { get; set; } = [];
    public IList<ShopItem> Sold { get; set; } = [];
        
    public Shop()
    {
        Name = "New shop";
        Currency = Item.List.ElementAt(0).Value;
    }

    public ShopItem FindBought(Item item) => Bought.First(x => x.Item == item);
}