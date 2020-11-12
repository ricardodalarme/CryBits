using CryBits.Entities;
using System;
using System.Collections.Generic;

namespace CryBits.Client.Entities
{
    class Shop : Entity
    {
        // Lista de dados
        public static Dictionary<Guid, Shop> List;

        // Obtém o dado, caso ele não existir retorna nulo
        public static Shop Get(Guid id) => List.ContainsKey(id) ? List[id] : null;

        // Dados gerais
        public Item Currency;
        public ShopItem[] Sold;
        public ShopItem[] Bought;

        public Shop(Guid id) : base(id) { }

        public ShopItem FindBought(Item item)
        {
            // Encontra um item especifico na lista de itens vendidos
            for (byte i = 0; i < Bought.Length; i++)
                if (Bought[i].Item.Equals(item))
                    return Bought[i];

            return null;
        }
    }

    class ShopItem
    {
        public Item Item;
        public short Amount;
        public short Price;
    }
}
