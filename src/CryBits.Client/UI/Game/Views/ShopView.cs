using CryBits.Client.Framework.UI.Entities;
using CryBits.Client.Rendering.UI;
using CryBits.Client.UI.Game.ViewModels;
using CryBits.Definitions.Shops;
using Iguina.Entities;
using Microsoft.Xna.Framework;

namespace CryBits.Client.UI.Game.Views;

internal class ShopView(
    UiContext uiContext,
    ItemIconRenderer itemRenderer,
    TooltipView tooltip,
    ShopViewModel viewModel) : ViewBase
{
    private Panel Panel => uiContext.Get<Panel>("Shop");
    private Button CloseButton => uiContext.Get<Button>("ShopClose");
    private Label NameLabel => uiContext.Get<Label>("ShopName");
    private Label CurrencyLabel => uiContext.Get<Label>("ShopCurrency");
    private SlotGrid Grid => uiContext.Get<SlotGrid>("ShopGrid");

    public bool TryGetSalePrice(Guid itemId, out short price)
    {
        price = 0;
        if (!Panel.Visible || viewModel.OpenedShop == null) return false;
        var bought = viewModel.OpenedShop.FindBought(itemId);
        if (bought == null) return false;
        price = bought.Price;
        return true;
    }

    public void Open(Shop shop)
    {
        if (shop == null) return;
        viewModel.Open(shop);
        NameLabel.Text = viewModel.Name;
        CurrencyLabel.Text = viewModel.CurrencyName;
        Panel.Visible = true;
        Bind();
    }

    public void Close()
    {
        Grid.ResetHover();
        tooltip.Hide();
        Panel.Visible = false;
        Unbind();
    }

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
        viewModel.Buy((short)slot);
    }

    private void OnClosePressed(Entity _)
    {
        viewModel.Close();
        Close();
    }

    private void OnSlotHoverEnter(int slot)
    {
        if (slot >= viewModel.SoldItems.Count) return;
        var itemVM = viewModel.SoldItems[slot];
        if (itemVM.Definition is not { } item) return;

        tooltip.Show(item,
            new Vector2(Panel.LastBoundingRect.X - 186, Panel.LastBoundingRect.Y + 5),
            "Price: " + itemVM.Price);
    }

    private void OnPostDraw()
    {
        if (!Panel.Visible || viewModel.OpenedShop == null) return;

        for (var i = 0; i < Grid.TotalSlots; i++)
        {
            if (i >= viewModel.SoldItems.Count) break;
            var rect = Grid.GetSlotRect(i);
            var itemVM = viewModel.SoldItems[i];
            if (itemVM.Definition is { } item)
                itemRenderer.DrawItem(item, itemVM.Amount, new Vector2(rect.X, rect.Y));
        }
    }
}
