using CryBits.Client.Framework.UI.Entities;
using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Network.Senders;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Shops;
using CryBits.Simulation.Intents;
using Iguina.Entities;
using System.Drawing;

namespace CryBits.Client.UI.Game.Views;

internal class ShopView(IguinaContext uiContext, IntentSender intentSender, ItemRenderer itemRenderer, DefinitionCatalog catalog) : ViewBase
{
    internal static Panel Panel => IguinaContext.Instance.Get<Panel>("Shop");
    private Button CloseButton => uiContext.Get<Button>("ShopClose");
    internal static Label NameLabel => IguinaContext.Instance.Get<Label>("ShopName");
    internal static Label CurrencyLabel => IguinaContext.Instance.Get<Label>("ShopCurrency");
    private SlotGrid Grid => uiContext.Get<SlotGrid>("ShopGrid");

    public static Shop? OpenedShop;

    public override void Bind()
    {
        Grid.OnSlotDoubleClick += OnSlotDoubleClick;
        Grid.OnSlotHoverEnter += OnSlotHoverEnter;
        Grid.OnSlotHoverLeave += TooltipView.Hide;
        CloseButton.Events.OnClick += OnClosePressed;
        uiContext.PostDraw += OnPostDraw;
    }

    public override void Unbind()
    {
        Grid.OnSlotDoubleClick -= OnSlotDoubleClick;
        Grid.OnSlotHoverEnter -= OnSlotHoverEnter;
        Grid.OnSlotHoverLeave -= TooltipView.Hide;
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
        TooltipView.Hide();
        Panel.Visible = false;
        intentSender.Send(new ShopCloseIntent(default));
    }

    private void OnSlotHoverEnter(int slot)
    {
        if (OpenedShop == null || slot >= OpenedShop.Sold.Count) return;
        var item = catalog.Items.Get(OpenedShop.Sold[slot].ItemId);
        if (item == null) return;
        TooltipView.Show(item.Id,
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

    public static void Open(Shop shop)
    {
        if (shop == null) return;
        OpenedShop = shop;
        NameLabel.Text = shop.Name;
        CurrencyLabel.Text = DefinitionCatalog.Instance.Items.Get(shop.CurrencyId)?.Name ?? "Unknown";
        Panel.Visible = true;
    }
}
