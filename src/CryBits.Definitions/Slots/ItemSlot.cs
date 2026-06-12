using CryBits.Definitions.Catalog;
using CryBits.Definitions.Items;
using CryBits.Definitions.Helpers.Extensions;
using System;
using System.Text.Json.Serialization;

namespace CryBits.Definitions.Slots;

[Serializable]
public class ItemSlot : ISlot
{
    private Guid _item;

    [JsonIgnore]
    public Item Item
    {
        get => DefinitionCatalog.Instance.Items.Get(_item);
        set => _item = value.GetId();
    }

    [JsonInclude]
    private Guid ItemId { get => _item; set => _item = value; }
    public short Amount { get; set; }

    public ItemSlot(Item item, short amount)
    {
        Item = item;
        Amount = amount;
    }

    public override string ToString() => Item.Name + " - " + Amount + "x";
}
