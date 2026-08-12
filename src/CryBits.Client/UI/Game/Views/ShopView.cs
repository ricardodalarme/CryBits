using CryBits.Client.Rendering.UI;
using CryBits.Client.UI.Game.ViewModels;
using CryBits.Definitions.Shops;
using Microsoft.Xna.Framework;
using Myra.Events;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;

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
    private Grid Grid => uiContext.Get<Grid>("ShopGrid");

    private readonly List<Image> _slotWidgets = new();

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
                Grid.SetColumn(img, c);
                Grid.SetRow(img, r);

                img.MouseEntered += (sender, e) => OnSlotHoverEnter(slotIndex);
                img.MouseLeft += (sender, e) => tooltip.Hide();
                img.TouchDoubleClick += (sender, e) => OnSlotDoubleClick(slotIndex);

                Grid.Widgets.Add(img);
                _slotWidgets.Add(img);
            }
        }
    }

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
        tooltip.Hide();
        Panel.Visible = false;
        Unbind();
    }

    public override void Bind()
    {
        EnsureSlotWidgets();
        CloseButton.Click += OnClosePressed;
        UpdateSlotIcons();
    }

    public override void Unbind()
    {
        CloseButton.Click -= OnClosePressed;
    }

    public void UpdateSlotIcons()
    {
        EnsureSlotWidgets();
        for (int i = 0; i < _slotWidgets.Count; i++)
        {
            if (i < viewModel.SoldItems.Count && viewModel.SoldItems[i].Definition is { } item)
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

    private void OnSlotDoubleClick(int slot)
    {
        viewModel.Buy((short)slot);
    }

    private void OnClosePressed(object? sender, MyraEventArgs e)
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
            new Vector2(Panel.Left, Panel.Top + 5),
            "Price: " + itemVM.Price);
    }
}
