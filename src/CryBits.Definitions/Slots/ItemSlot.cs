using CryBits.Definitions.Catalog;
using CryBits.Definitions.Items;
using CryBits.Definitions.Helpers.Extensions;
using System;

namespace CryBits.Definitions.Slots;

[Serializable]
public class ItemSlot : ISlot
{
    private Guid _item;
    public Item Item
    {
        get => DefinitionCatalog.Items.Get(_item);
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
