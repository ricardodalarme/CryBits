using CryBits.Client.Rendering.UI;
using CryBits.Client.UI.Game.ViewModels;
using CryBits.Definitions.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using MyraGrid = Myra.Graphics2D.UI.Grid;

namespace CryBits.Client.UI.Game.Views;

internal class InventoryView(
    UiContext uiContext,
    ItemIconRenderer itemRenderer,
    TooltipView tooltip,
    ShopView shop,
    GameScreen gameScreen,
    InventoryViewModel viewModel) : ViewBase
{
    internal InventoryViewModel ViewModel => viewModel;

    private MyraGrid Grid => uiContext.Get<MyraGrid>("InventoryGrid");
    private readonly List<Image> _slotWidgets = new();

    internal short? DragOrigin { get; private set; }

    public void ClearDrag() => DragOrigin = null;

    private void EnsureSlotWidgets()
    {
        if (_slotWidgets.Count > 0) return;

        int cols = 5;
        int rows = 4;
        int slotSize = 32;
        int spacing = 4;

        Grid.ColumnsProportions.Clear();
        for (int c = 0; c < cols; c++)
            Grid.ColumnsProportions.Add(new Proportion(ProportionType.Pixels, slotSize));

        Grid.RowsProportions.Clear();
        for (int r = 0; r < rows; r++)
            Grid.RowsProportions.Add(new Proportion(ProportionType.Pixels, slotSize));

        Grid.ColumnSpacing = spacing;
        Grid.RowSpacing = spacing;
        Grid.Widgets.Clear();

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                int slotIndex = r * cols + c;
                var img = new Image
                {
                    Width = slotSize,
                    Height = slotSize,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                MyraGrid.SetColumn(img, c);
                MyraGrid.SetRow(img, r);

                img.TouchDown += (sender, e) => OnSlotTouchDown(slotIndex);
                img.TouchUp += (sender, e) => OnSlotTouchUp(slotIndex);
                img.TouchDoubleClick += (sender, e) => OnSlotDoubleClick(slotIndex);
                img.MouseEntered += (sender, e) => OnSlotHoverEnter(slotIndex);
                img.MouseLeft += (sender, e) => tooltip.Hide();

                Grid.Widgets.Add(img);
                _slotWidgets.Add(img);
            }
        }
    }

    public override void Bind()
    {
        EnsureSlotWidgets();
        UpdateSlotIcons();
    }

    public override void Unbind()
    {
        tooltip.Hide();
    }

    public void UpdateSlotIcons()
    {
        EnsureSlotWidgets();
        for (int i = 0; i < _slotWidgets.Count && i < viewModel.Slots.Length; i++)
        {
            var itemVM = viewModel.Slots[i];
            if (itemVM != null && itemVM.ItemId != Guid.Empty && itemVM.Definition != null)
            {
                var tex = itemRenderer.GetTexture(itemVM.Definition);
                _slotWidgets[i].Renderable = tex != null ? new TextureRegion(tex) : null;
            }
            else
            {
                _slotWidgets[i].Renderable = null;
            }
        }
    }

    private void OnSlotTouchDown(int slot)
    {
        var mouse = Mouse.GetState();
        if (mouse.RightButton == ButtonState.Pressed)
        {
            OnSlotRightClick(slot);
            return;
        }

        var item = viewModel.Slots[slot];
        if (item == null || item.ItemId == Guid.Empty) return;

        DragOrigin = (short)slot;
        gameScreen.InventoryChange = (short)slot;
    }

    private void OnSlotTouchUp(int slot)
    {
        var dragSlot = DragOrigin;
        DragOrigin = null;
        gameScreen.InventoryChange = null;
        if (dragSlot == null) return;

        viewModel.Swap(dragSlot.Value, (short)slot);
        if (uiContext.Registry.TryGetValue("Drop", out var dropWidget))
            dropWidget.Visible = false;
    }

    private void OnSlotRightClick(int slot)
    {
        var itemVM = viewModel.Slots[slot];
        if (itemVM == null || itemVM.ItemId == Guid.Empty) return;

        var item = itemVM.Definition;
        if (item?.Bind == BindOn.Pickup) return;

        if (uiContext.Registry.TryGetValue("Shop", out var shopWidget) && shopWidget.Visible)
        {
            if (itemVM.Amount != 1)
            {
                gameScreen.ShopSellView.Show((short)slot);
                gameScreen.ShopSellView.AmountInput.Value = 1;
            }
            else
            {
                viewModel.Sell((short)slot, 1);
            }
        }
        else if (!uiContext.Registry.TryGetValue("Trade", out var tradeWidget) || !tradeWidget.Visible)
        {
            if (itemVM.Amount != 1)
            {
                gameScreen.DropItemView.Show((short)slot);
                gameScreen.DropItemView.AmountInput.Value = 1;
            }
            else
            {
                viewModel.Drop((short)slot, 1);
            }
        }
    }

    private void OnSlotDoubleClick(int slot)
    {
        viewModel.Use((short)slot);
        if (uiContext.Registry.TryGetValue("Drop", out var dropWidget))
            dropWidget.Visible = false;
    }

    private void OnSlotHoverEnter(int slot)
    {
        var itemVM = viewModel.Slots[slot];
        if (itemVM == null || itemVM.ItemId == Guid.Empty) return;
        var item = itemVM.Definition;
        if (item == null) return;

        string? additionalInfo = null;
        if (shop.TryGetSalePrice(item.Id, out var price))
            additionalInfo = "Sale price: " + price;

        var panel = uiContext.Get<Panel>("InventoryPanel");
        tooltip.Show(item, new Vector2(panel.Left, panel.Top + 3), additionalInfo);
    }
}
