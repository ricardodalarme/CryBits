using System;
using CryBits.Interfaces;

namespace CryBits.Entities.Slots;

[Serializable]
public class ItemSlot : ISlot
{
    private Guid _item;
    public Item Item
    {
        get => Item.List.Get(_item);
        set => _item = value.GetId();
    }
    public short Amount { get; set; }

    public ItemSlot(Item item, short amount)
    {
        Item = item;
        Amount = amount;
    }

    public override string ToString() => Item.Name + " - " + Amount + "x";
}