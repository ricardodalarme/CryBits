using System.Drawing;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Network.Senders;
using CryBits.Entities.Shop;
using SFML.Window;

namespace CryBits.Client.UI.Game.Views;

internal class ShopView(ShopSender shopSender, ItemRenderer itemRenderer) : IView
{
    internal static Panel Panel => Tools.Panels["Shop"];
    private static Button CloseButton => Tools.Buttons["Shop_Close"];
    private static Label NameLabel => Tools.Labels["Shop_Name"];
    private static Label CurrencyLabel => Tools.Labels["Shop_Currency"];
    private static SlotGrid Grid => Tools.SlotGrids["Shop_Grid"];

    public static Shop OpenedShop;

    public void Bind()
    {
        Grid.OnRenderSlot += OnRenderSlot;
        Grid.OnMouseDoubleClick += OnGridMouseDoubleClick;
        CloseButton.OnMouseUp += OnClosePressed;
    }

    public void Unbind()
    {
        Grid.OnRenderSlot -= OnRenderSlot;
        Grid.OnMouseDoubleClick -= OnGridMouseDoubleClick;
        CloseButton.OnMouseUp -= OnClosePressed;
    }

    private void OnRenderSlot(int slot, Point pos)
    {
        if (OpenedShop == null || slot >= OpenedShop.Sold.Count) return;
        itemRenderer.DrawItem(OpenedShop.Sold[slot].Item, OpenedShop.Sold[slot].Amount, pos);
    }

    private void OnGridMouseDoubleClick(MouseButtonEventArgs e, short slot)
    {
        if (OpenedShop == null) return;

        // Purchase shop item.
        shopSender.ShopBuy((byte)slot);
    }

    private void OnClosePressed()
    {
        Panel.Visible = false;
        shopSender.ShopClose();
    }

    public static void Open(Shop shop)
    {
        OpenedShop = shop;
        NameLabel.SetArguments(shop.Name);
        CurrencyLabel.SetArguments(shop.Currency.Name);
        Panel.Visible = true;
    }
}
