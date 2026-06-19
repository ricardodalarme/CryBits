using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Network.Senders;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Shops;
using SFML.Window;
using System.Drawing;

namespace CryBits.Client.UI.Game.Views;

internal class ShopView(ShopSender shopSender, ItemRenderer itemRenderer, DefinitionCatalog catalog) : IView
{
    private readonly DefinitionCatalog _catalog = catalog;
    internal static Panel Panel => Tools.Panels["Shop"];
    private static Button CloseButton => Tools.Buttons["Shop_Close"];
    private static Label NameLabel => Tools.Labels["Shop_Name"];
    private static Label CurrencyLabel => Tools.Labels["Shop_Currency"];
    private static SlotGrid Grid => Tools.SlotGrids["Shop_Grid"];

    public static Shop? OpenedShop;

    public void Bind()
    {
        Grid.OnRenderSlot += OnRenderSlot;
        Grid.OnMouseDoubleClick += OnGridMouseDoubleClick;
        Grid.OnSlotHover += OnGridSlotHover;
        Grid.OnSlotLeave += OnGridSlotLeave;
        CloseButton.OnMouseUp += OnClosePressed;
    }

    public void Unbind()
    {
        Grid.OnRenderSlot -= OnRenderSlot;
        Grid.OnMouseDoubleClick -= OnGridMouseDoubleClick;
        Grid.OnSlotHover -= OnGridSlotHover;
        Grid.OnSlotLeave -= OnGridSlotLeave;
        CloseButton.OnMouseUp -= OnClosePressed;
    }

    private void OnRenderSlot(int slot, Point pos)
    {
        if (OpenedShop == null || slot >= OpenedShop.Sold.Count) return;
        if (_catalog.Items.Get(OpenedShop.Sold[slot].ItemId) is { } item)
            itemRenderer.DrawItem(item, OpenedShop.Sold[slot].Amount, pos);
    }

    private void OnGridMouseDoubleClick(MouseButtonEventArgs e, short slot)
    {
        if (OpenedShop == null) return;

        // Purchase shop item.
        shopSender.ShopBuy((byte)slot);
    }

    private void OnClosePressed()
    {
        Grid.ResetHover();
        InformationView.Hide();
        Panel.Visible = false;
        shopSender.ShopClose();
    }

    private void OnGridSlotHover(short slot)
    {
        if (OpenedShop == null || slot >= OpenedShop.Sold.Count) return;
        var item = _catalog.Items.Get(OpenedShop.Sold[slot].ItemId);
        if (item == null) return;
        InformationView.Show(item.Id,
            new Point(Panel.Position.X - 186, Panel.Position.Y + 5),
            "Price: " + OpenedShop.Sold[slot].Price);
    }

    private static void OnGridSlotLeave(short slot) => InformationView.Hide();

    public static void Open(Shop shop)
    {
        if (shop == null) return;
        OpenedShop = shop;
        NameLabel.SetArguments(shop.Name);
        CurrencyLabel.SetArguments(DefinitionCatalog.Instance.Items.Get(shop.CurrencyId)?.Name ?? "Unknown");
        Panel.Visible = true;
    }
}
