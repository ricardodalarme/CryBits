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
        public static Shop Get(Guid id) => List.ContainsKey(id) ? List[id] : null;

        // Dados
        private Guid _currency;
        public Item Currency
        {
            get => Item.Get(_currency);
            set => _currency = new Guid(value.GetID());
        }
        public IList<ShopItem> Bought { get; set; } = new List<ShopItem>();
        public IList<ShopItem> Sold { get; set; } = new List<ShopItem>();

        // Construtor
        public Shop(Guid id) : base(id) { }

        public ShopItem FindBought(Item item)
        {
            // Verifica se a loja vende determinado item
            for (byte i = 0; i < Bought.Count; i++)
                if (Bought[i].Item == item)
                    return Bought[i];

            return null;
        }
    }

    [Serializable]
    public class ShopItem : ItemSlot
    {
        public short Price { get; set; }

        public ShopItem(Item item, short amount, short price) : base(item, amount)
        {
            Price = price;
        }

        public override string ToString() => Item.Name + " - " + Amount + "x [$" + Price + "]";
    }
}
