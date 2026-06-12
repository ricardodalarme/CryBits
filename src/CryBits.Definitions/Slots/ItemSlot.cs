using System;

namespace CryBits.Definitions.Slots;

[Serializable]
public class ItemSlot : ISlot
{
    public Guid ItemId { get; set; }
    public short Amount { get; set; }

    public ItemSlot(Guid itemId, short amount)
    {
        ItemId = itemId;
        Amount = amount;
    }

    public override string ToString() => ItemId + " - " + Amount + "x";
}
