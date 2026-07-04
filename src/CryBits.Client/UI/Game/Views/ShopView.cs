using CryBits.Client.Framework.UI.Entities;
using CryBits.Client.Rendering.UI;
using CryBits.Client.UI.Game.ViewModels;
using CryBits.Definitions.Shops;
using Iguina.Entities;
using System.Drawing;

namespace CryBits.Client.UI.Game.Views;

internal class ShopView(
    UiContext uiContext,
    ItemIconRenderer itemRenderer,
    TooltipView tooltip,
    ShopViewModel viewModel) : ViewBase
{
    public ShopViewModel ViewModel => viewModel;

    internal Panel Panel => uiContext.Get<Panel>("Shop");
    private Button CloseButton => uiContext.Get<Button>("ShopClose");
    internal Label NameLabel => uiContext.Get<Label>("ShopName");
    internal Label CurrencyLabel => uiContext.Get<Label>("ShopCurrency");
    private SlotGrid Grid => uiContext.Get<SlotGrid>("ShopGrid");

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
        ViewModel.Buy((short)slot);
    }

    private void OnClosePressed(Entity _)
    {
        Grid.ResetHover();
        tooltip.Hide();
        Panel.Visible = false;
        ViewModel.Close();
    }

    private void OnSlotHoverEnter(int slot)
    {
        if (slot >= ViewModel.SoldItems.Count) return;
        var itemVM = ViewModel.SoldItems[slot];
        if (itemVM.Definition == null) return;

        tooltip.Show(itemVM.ItemId,
            new Point(Panel.LastBoundingRect.X - 186, Panel.LastBoundingRect.Y + 5),
            "Price: " + itemVM.Price);
    }

    private void OnPostDraw()
    {
        if (!Panel.Visible || ViewModel.OpenedShop == null) return;

        for (var i = 0; i < Grid.TotalSlots; i++)
        {
            if (i >= ViewModel.SoldItems.Count) break;
            var rect = Grid.GetSlotRect(i);
            var itemVM = ViewModel.SoldItems[i];
            if (itemVM.Definition is { } item)
                itemRenderer.DrawItem(item, itemVM.Amount, new Point(rect.X, rect.Y));
        }
    }

    public void Open(Shop shop)
    {
        if (shop == null) return;
        ViewModel.Open(shop);
        NameLabel.Text = ViewModel.Name;
        CurrencyLabel.Text = ViewModel.CurrencyName;
        Panel.Visible = true;
    }
}
