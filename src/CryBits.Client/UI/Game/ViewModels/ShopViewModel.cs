using CryBits.Client.Network.Senders;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Items;
using CryBits.Definitions.Shops;
using CryBits.Simulation.Intents;

namespace CryBits.Client.UI.Game.ViewModels;

internal sealed class ShopSoldItemViewModel
{
    public short Index { get; set; }
    public Guid ItemId { get; set; }
    public int Price { get; set; }
    public short Amount { get; set; }
    public Item? Definition { get; set; }
}

internal sealed class ShopViewModel(
    IntentSender intentSender,
    DefinitionCatalog catalog)
{
    public Shop? OpenedShop { get; private set; }
    public List<ShopSoldItemViewModel> SoldItems { get; private set; } = [];
    public string Name => OpenedShop?.Name ?? string.Empty;

    public string CurrencyName =>
        OpenedShop != null ? catalog.Items.Get(OpenedShop.CurrencyId)?.Name ?? "Unknown" : string.Empty;

    public void Open(Shop shop)
    {
        OpenedShop = shop;
        SoldItems = shop.Sold.Select((sold, idx) => new ShopSoldItemViewModel
        {
            Index = (short)idx,
            ItemId = sold.ItemId,
            Price = sold.Price,
            Amount = sold.Amount,
            Definition = catalog.Items.Get(sold.ItemId)
        }).ToList();
    }

    public void Close()
    {
        OpenedShop = null;
        SoldItems = [];
        intentSender.Send(new ShopCloseIntent(default));
    }

    public void Buy(short slot)
    {
        if (OpenedShop != null)
            intentSender.Send(new ShopBuyIntent(default, (byte)slot));
    }

    public void Sell(short inventorySlot, short amount)
    {
        intentSender.Send(new ShopSellIntent(default, (byte)inventorySlot, amount));
    }
}
