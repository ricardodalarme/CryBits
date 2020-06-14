using System;
using System.Collections.Generic;

namespace Objects
{
    class Shop : Data
    {
        // Lista de dados
        public static Dictionary<Guid, Shop> List ;

        // Obtém o dado, caso ele não existir retorna nulo
        public static Shop Get(Guid ID) => List.ContainsKey(ID) ? List[ID] : null;

        // Dados gerais
        public string Name;
        public Item Currency;
        public Shop_Item[] Sold;
        public Shop_Item[] Bought;

        public Shop(Guid ID) : base(ID) { }

        public Shop_Item FindBought(Item Item)
        {
            // Encontra um item especifico na lista de itens vendidos
            for (byte i = 0; i < Bought.Length; i++)
                if (Bought[i].Item.Equals(Item))
                    return Bought[i];

            return null;
        }
    }

    class Shop_Item
    {
        public Item Item;
        public short Amount;
        public short Price;
    }
}
