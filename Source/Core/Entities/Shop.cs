using System;
using System.Collections.Generic;

namespace CryBits.Entities
{
    [Serializable]
    public class Shop : Entity
    {
        // Lista de dados
        public static Dictionary<Guid, Shop> List = new Dictionary<Guid, Shop>();

        // Obtém o dado, caso ele não existir retorna nulo
        public static Shop Get(Guid ID) => List.ContainsKey(ID) ? List[ID] : null;

        // Dados
        private Guid currency;
        public Item Currency
        {
            get => Item.Get(currency);
            set => currency = new Guid(value.GetID());
        }
        public IList<Shop_Item> Bought { get; set; } = Array.Empty<Shop_Item>();
        public IList<Shop_Item> Sold { get; set; } = Array.Empty<Shop_Item>();

        // Construtor
        public Shop(Guid ID) : base(ID) { }

        public Shop_Item FindBought(Item Item)
        {
            // Verifica se a loja vende determinado item
            for (byte i = 0; i < Bought.Count; i++)
                if (Bought[i].Item == Item)
                    return Bought[i];

            return null;
        }
    }

    [Serializable]
    public class Shop_Item : ItemSlot
    {
        public short Price { get; set; }

        public Shop_Item(Item Item, short Amount, short Price) : base(Item, Amount)
        {
            this.Price = Price;
        }

        public override string ToString() => Item.Name + " - " + Amount + "x [$" + Price + "]";
    }
}
