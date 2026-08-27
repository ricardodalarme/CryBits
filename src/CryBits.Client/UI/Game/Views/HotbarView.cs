using CryBits.Client.Rendering.UI;
using CryBits.Client.UI.Game.ViewModels;
using CryBits.Definitions.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;

namespace CryBits.Client.UI.Game.Views;

internal class HotbarView(
    UiContext uiContext,
    ItemIconRenderer itemRenderer,
    TooltipView tooltip,
    InventoryView inventory,
    GameScreen gameScreen,
    HotbarViewModel viewModel) : ViewBase
{
    private Grid Grid => uiContext.Get<Grid>("HotbarGrid");
    private readonly List<Image> _slotWidgets = new();

    private short? _hotbarDragOrigin;

    private void EnsureSlotWidgets()
    {
        if (_slotWidgets.Count > 0) return;

        int cols = 10;
        int rows = 1;
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

        for (int c = 0; c < cols; c++)
        {
            int slotIndex = c;
            var img = new Image
            {
                Width = slotSize,
                Height = slotSize,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(img, c);
            Grid.SetRow(img, 0);

            img.TouchDown += (sender, e) => OnSlotTouchDown(slotIndex);
            img.TouchUp += (sender, e) => OnSlotTouchUp(slotIndex);
            img.MouseEntered += (sender, e) => OnSlotHoverEnter(slotIndex);
            img.MouseLeft += (sender, e) => tooltip.Hide();

            Grid.Widgets.Add(img);
            _slotWidgets.Add(img);
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
            var hbSlot = viewModel.Slots[i];
            if (hbSlot is { Slot: > 0, Type: SlotType.Item } && hbSlot.Definition is { } item)
            {
                var tex = itemRenderer.GetTexture(item);
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

        var hbSlot = viewModel.Slots[slot];
        if (hbSlot is not { Slot: > 0 }) return;

        _hotbarDragOrigin = (short)slot;
        gameScreen.HotbarChange = (short)slot;
    }

    private void OnSlotTouchUp(int slot)
    {
        var hotSlot = _hotbarDragOrigin;
        _hotbarDragOrigin = null;
        gameScreen.HotbarChange = null;
        gameScreen.InventoryChange = null;
        if (hotSlot is not null)
            viewModel.Swap(hotSlot.Value, (short)slot);

        var invSlot = inventory.DragOrigin;
        if (invSlot is not null)
            viewModel.AddItem((short)slot, invSlot.Value);
    }

    private void OnSlotRightClick(int slot)
    {
        viewModel.Remove((short)slot);
    }

    private void OnSlotHoverEnter(int slot)
    {
        var hbSlot = viewModel.Slots[slot];
        if (hbSlot is { Slot: > 0, Type: SlotType.Item } && hbSlot.Definition is { } item)
        {
            var panel = uiContext.Get<Panel>("HotbarPanel");
            tooltip.Show(item, new Vector2(panel.Left, panel.Top + 42));
        }
    }
}
