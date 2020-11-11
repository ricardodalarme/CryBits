using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CryBits.Editors.Entities
{
    class Shop : Entity
    {
        // Lista de dados
        public static Dictionary<Guid, Shop> List = new Dictionary<Guid, Shop>();

        // Obtém o dado, caso ele não existir retorna nulo
        public static Shop Get(Guid ID) => List.ContainsKey(ID) ? List[ID] : null;

        // Dados
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
