using System;

namespace Objects
{
    class Shop : Lists.Structures.Data
    {
        public string Name;
        public Item Currency;
        public Shop_Item[] Sold;
        public Shop_Item[] Bought;

        public Shop(Guid ID) : base(ID) { }

        public Shop_Item FindBought(Item Item)
        {
            // Encontra um item especifico na lista de itens vendidos
            for (byte i = 0; i < Utils.Shop_Open.Bought.Length; i++)
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
