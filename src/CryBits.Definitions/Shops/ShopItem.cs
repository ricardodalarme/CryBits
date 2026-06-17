namespace CryBits.Definitions.Shops;

[Serializable]
public class ShopItem(Guid itemId, short amount, short price)
{
    public Guid ItemId { get; set; } = itemId;
    public short Amount { get; set; } = amount;
    public short Price { get; set; } = price;

    public override string ToString() => ItemId + " - " + Amount + "x [$" + Price + "]";
}
