using System;

namespace CryBits.Entities
{
    public interface ISlot
    {
        public short Amount { get; set; }
    }

    [Serializable]
    public class ItemSlot : ISlot
    {
        private Guid _item;
        public Item Item
        {
            get => Item.Get(_item);
            set => _item = new Guid(value.GetID());
        }
        public short Amount { get; set; }

        public ItemSlot(Item item, short amount)
        {
            Item = item;
            Amount = amount;
        }

        public override string ToString() => Item.Name + " - " + Amount + "x";
    }
}
