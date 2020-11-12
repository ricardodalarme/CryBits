namespace CryBits.Entities
{
    public interface ISlot
    {
        public short Amount { get; set; }
    }

    public class ItemSlot : ISlot
    {
        public Item Item { get; set; }
        public short Amount { get; set; }

        public ItemSlot(Item Item, short Amount)
        {
            this.Item = Item;
            this.Amount = Amount;
        }

        public override string ToString() => Item.Name + " - " + Amount + "x";
    }
}
