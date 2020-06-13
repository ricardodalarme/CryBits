using System;
using System.Collections.Generic;
using static Utils;

namespace Objects
{
    [Serializable]
    class Shop : Data
    {
        // Lista de dados
        public static Dictionary<Guid, Shop> List = new Dictionary<Guid, Shop>();

        // Obtém o dado, caso ele não existir retorna nulo
        public static Shop Get(Guid ID) => List.ContainsKey(ID) ? List[ID] : null;

        // Dados
        public string Name;
        private Guid currency;
        public Item Currency
        {
            get => Item.Get( currency);
            set => currency = new Guid(GetID(value));
        }
        public Shop_Item[] Bought = Array.Empty<Shop_Item>();
        public Shop_Item[] Sold = Array.Empty<Shop_Item>();

        // Construtor
        public Shop(Guid ID) : base(ID) { }

        public Shop_Item BoughtItem(Item Item)
        {
            // Verifica se a loja vende determinado item
            for (byte i = 0; i < Bought.Length; i++)
                if (Bought[i].Item == Item)
                    return Bought[i];

            return null;
        }
    }

    [Serializable]
    class Shop_Item
    {
        private Guid item;
        public Item Item
        {
            get => Item.Get( item);
            set => item = new Guid(GetID(value));
        }
        public short Amount;
        public short Price;
    }
}
