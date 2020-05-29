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
    }

    class Shop_Item
    {
        public Item Item;
        public short Amount;
        public short Price;
    }
}
