using System;
using CryBits.Enums;

namespace CryBits.Entities;

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
        get => Item.List.Get(_item);
        set => _item = value.GetID();
    }
    public short Amount { get; set; }

    public ItemSlot(Item item, short amount)
    {
        Item = item;
        Amount = amount;
    }

    public override string ToString() => Item.Name + " - " + Amount + "x";
}

public class TradeSlot : ISlot
{
    public short SlotNum { get; set; }
    public short Amount { get; set; }
}

public class HotbarSlot
{
    public SlotType Type;
    public short Slot;

    public HotbarSlot(SlotType type, short slot)
    {
        Type = type;
        Slot = slot;
    }
}