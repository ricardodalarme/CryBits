using System;

namespace Objects
{
    [Serializable]
    class Shop : Data
    {
        // Dados
        public string Name;
        private Guid currency;
        public Item Currency
        {
            get => (Item)Lists.GetData(Lists.Item, currency);
            set => currency = new Guid(Lists.GetID(value));
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
            get => (Item)Lists.GetData(Lists.Item, item);
            set => item = new Guid(Lists.GetID(value));
        }
        public short Amount;
        public short Price;
    }
}
