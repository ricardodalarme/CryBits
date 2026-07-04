using CryBits.Client.Framework.UI.Entities;
using CryBits.Client.Network.Senders;
using CryBits.Client.Rendering.UI;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Shops;
using CryBits.Simulation.Intents;
using Iguina.Entities;
using System.Drawing;

namespace CryBits.Client.UI.Game.Views;

internal class ShopView(UiContext uiContext, IntentSender intentSender, ItemIconRenderer itemRenderer, DefinitionCatalog catalog, TooltipView tooltip) : ViewBase
{
    internal Panel Panel => uiContext.Get<Panel>("Shop");
    private Button CloseButton => uiContext.Get<Button>("ShopClose");
    internal Label NameLabel => uiContext.Get<Label>("ShopName");
    internal Label CurrencyLabel => uiContext.Get<Label>("ShopCurrency");
    private SlotGrid Grid => uiContext.Get<SlotGrid>("ShopGrid");

    public Shop? OpenedShop;

    public override void Bind()
    {
        Grid.OnSlotDoubleClick += OnSlotDoubleClick;
        Grid.OnSlotHoverEnter += OnSlotHoverEnter;
        Grid.OnSlotHoverLeave += tooltip.Hide;
        CloseButton.Events.OnClick += OnClosePressed;
        uiContext.PostDraw += OnPostDraw;
    }

    public override void Unbind()
    {
        Grid.OnSlotDoubleClick -= OnSlotDoubleClick;
        Grid.OnSlotHoverEnter -= OnSlotHoverEnter;
        Grid.OnSlotHoverLeave -= tooltip.Hide;
        CloseButton.Events.OnClick -= OnClosePressed;
        uiContext.PostDraw -= OnPostDraw;
    }

    private void OnSlotDoubleClick(int slot)
    {
        if (OpenedShop != null)
            intentSender.Send(new ShopBuyIntent(default, (byte)slot));
    }

    private void OnClosePressed(Entity _)
    {
        Grid.ResetHover();
        tooltip.Hide();
        Panel.Visible = false;
        intentSender.Send(new ShopCloseIntent(default));
    }

    private void OnSlotHoverEnter(int slot)
    {
        if (OpenedShop == null || slot >= OpenedShop.Sold.Count) return;
        var item = catalog.Items.Get(OpenedShop.Sold[slot].ItemId);
        if (item == null) return;
        tooltip.Show(item.Id,
            new Point(Panel.LastBoundingRect.X - 186, Panel.LastBoundingRect.Y + 5),
            "Price: " + OpenedShop.Sold[slot].Price);
    }

    private void OnPostDraw()
    {
        if (!Panel.Visible || OpenedShop == null) return;

        for (var i = 0; i < Grid.TotalSlots; i++)
        {
            if (i >= OpenedShop.Sold.Count) break;
            var rect = Grid.GetSlotRect(i);
            if (catalog.Items.Get(OpenedShop.Sold[i].ItemId) is { } item)
                itemRenderer.DrawItem(item, OpenedShop.Sold[i].Amount, new Point(rect.X, rect.Y));
        }
    }

    public void Open(Shop shop)
    {
        if (shop == null) return;
        OpenedShop = shop;
        NameLabel.Text = shop.Name;
        CurrencyLabel.Text = catalog.Items.Get(shop.CurrencyId)?.Name ?? "Unknown";
        Panel.Visible = true;
    }
}
