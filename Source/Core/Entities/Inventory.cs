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
        public Item Item { get; set; }
        public short Amount { get; set; }

        public ItemSlot(Item item, short amount)
        {
            this.Item = item;
            this.Amount = amount;
        }

        public override string ToString() => Item.Name + " - " + Amount + "x";
    }
}
