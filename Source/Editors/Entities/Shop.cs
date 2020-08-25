using System;
using System.ComponentModel;

namespace Entities
{
    class Shop : Entity
    {
        public string Name = string.Empty;
        public Item Currency;
        public BindingList<Shop_Item> Sold = new BindingList<Shop_Item>();
        public BindingList<Shop_Item> Bought = new BindingList<Shop_Item>();

        public Shop(Guid ID) : base(ID) { }
        public override string ToString() => Name;
    }

    class Shop_Item : Lists.Structures.Inventory
    {
        public short Price;

        public Shop_Item(Item Item, short Amount, short Price) : base(Item, Amount)
        {
            this.Price = Price;
        }

        public override string ToString() => Item.Name + " - " + Amount + "x [$" + Price + "]";
    }
}
