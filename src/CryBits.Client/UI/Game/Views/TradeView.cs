using CryBits.Client.Rendering.UI;
using CryBits.Client.UI.Game.ViewModels;
using CryBits.Definitions.Common;
using Microsoft.Xna.Framework.Input;
using Myra.Events;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;

namespace CryBits.Client.UI.Game.Views;

internal class TradeView(
    UiContext uiContext,
    ItemIconRenderer itemRenderer,
    InventoryView inventory,
    GameScreen gameScreen,
    TradeViewModel viewModel) : ViewBase
{
    private Panel Panel => uiContext.Get<Panel>("Trade");
    private Panel OfferDisabledPanel => uiContext.Get<Panel>("TradeOfferDisable");
    private Button CloseButton => uiContext.Get<Button>("TradeClose");
    private Button AcceptOfferButton => uiContext.Get<Button>("TradeAccept");
    private Button DeclineOfferButton => uiContext.Get<Button>("TradeDecline");
    private Button ConfirmOfferButton => uiContext.Get<Button>("TradeConfirm");
    private Grid OwnGrid => uiContext.Get<Grid>("TradeGridOwn");
    private Grid TheirGrid => uiContext.Get<Grid>("TradeGridTheir");

    private readonly List<Image> _ownSlotWidgets = new();
    private readonly List<Image> _theirSlotWidgets = new();

    private short _ownSlot;
    private short _inventorySlot;

    private void EnsureSlotWidgets()
    {
        if (_ownSlotWidgets.Count > 0) return;

        int cols = 4;
        int rows = 3;
        int slotSize = 32;
        int spacing = 4;

        SetupGrid(OwnGrid, _ownSlotWidgets, cols, rows, slotSize, spacing, isOwnGrid: true);
        SetupGrid(TheirGrid, _theirSlotWidgets, cols, rows, slotSize, spacing, isOwnGrid: false);
    }

    private void SetupGrid(Grid grid, List<Image> list, int cols, int rows, int slotSize, int spacing, bool isOwnGrid)
    {
        grid.ColumnsProportions.Clear();
        for (int c = 0; c < cols; c++)
            grid.ColumnsProportions.Add(new Proportion(ProportionType.Pixels, slotSize));

        grid.RowsProportions.Clear();
        for (int r = 0; r < rows; r++)
            grid.RowsProportions.Add(new Proportion(ProportionType.Pixels, slotSize));

        grid.ColumnSpacing = spacing;
        grid.RowSpacing = spacing;
        grid.Widgets.Clear();

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

                if (isOwnGrid)
                {
                    img.TouchDown += (sender, e) => OnOwnSlotTouchDown(slotIndex);
                    img.TouchUp += (sender, e) => OnOwnSlotLeftUp(slotIndex);
                }

                grid.Widgets.Add(img);
                list.Add(img);
            }
        }
    }

    public void Open(bool activeState)
    {
        viewModel.IsOpen = activeState;

        if (activeState)
        {
            Panel.Visible = true;
            ConfirmOfferButton.Visible = true;
            AcceptOfferButton.Visible = DeclineOfferButton.Visible = false;
            OfferDisabledPanel.Visible = false;

            viewModel.ResetOffers();
            Bind();
        }
        else
        {
            Close();
        }
    }

    public void Close()
    {
        Panel.Visible = false;
        ConfirmOfferButton.Visible = true;
        AcceptOfferButton.Visible = false;
        DeclineOfferButton.Visible = false;
        OfferDisabledPanel.Visible = false;
        Unbind();
    }

    public void SetStatus(TradeStatus status)
    {
        switch (status)
        {
            case TradeStatus.Accepted:
            case TradeStatus.Declined:
                ConfirmOfferButton.Visible = true;
                AcceptOfferButton.Visible = DeclineOfferButton.Visible = false;
                OfferDisabledPanel.Visible = false;
                break;
            case TradeStatus.Confirmed:
                ConfirmOfferButton.Visible = false;
                AcceptOfferButton.Visible = DeclineOfferButton.Visible = true;
                OfferDisabledPanel.Visible = false;
                break;
        }
    }

    public override void Bind()
    {
        EnsureSlotWidgets();
        CloseButton.Click += OnClosePressed;
        AcceptOfferButton.Click += OnAcceptOfferPressed;
        DeclineOfferButton.Click += OnDeclineOfferPressed;
        ConfirmOfferButton.Click += OnConfirmPressed;
        UpdateSlotIcons();
    }

    public override void Unbind()
    {
        CloseButton.Click -= OnClosePressed;
        AcceptOfferButton.Click -= OnAcceptOfferPressed;
        DeclineOfferButton.Click -= OnDeclineOfferPressed;
        ConfirmOfferButton.Click -= OnConfirmPressed;
    }

    public void UpdateSlotIcons()
    {
        EnsureSlotWidgets();
        var ownOffer = viewModel.OwnOffer;
        var theirOffer = viewModel.TheirOffer;

        for (int i = 0; i < _ownSlotWidgets.Count; i++)
        {
            if (ownOffer != null && i < ownOffer.Length && ownOffer[i]?.Definition is { } item)
            {
                var tex = itemRenderer.GetTexture(item);
                _ownSlotWidgets[i].Renderable = tex != null ? new TextureRegion(tex) : null;
            }
            else
            {
                _ownSlotWidgets[i].Renderable = null;
            }
        }

        for (int i = 0; i < _theirSlotWidgets.Count; i++)
        {
            if (theirOffer != null && i < theirOffer.Length && theirOffer[i]?.Definition is { } item)
            {
                var tex = itemRenderer.GetTexture(item);
                _theirSlotWidgets[i].Renderable = tex != null ? new TextureRegion(tex) : null;
            }
            else
            {
                _theirSlotWidgets[i].Renderable = null;
            }
        }
    }

    private void OnOwnSlotTouchDown(int slot)
    {
        var mouse = Mouse.GetState();
        if (mouse.RightButton == ButtonState.Pressed)
        {
            OnOwnSlotRightClick(slot);
        }
    }

    private void OnOwnSlotRightClick(int slot)
    {
        if (!Panel.Visible) return;
        viewModel.RemoveOfferItem((short)slot);
    }

    private void OnOwnSlotLeftUp(int slot)
    {
        gameScreen.InventoryChange = null;
        var invSlot = inventory.DragOrigin;
        if (invSlot == null) return;

        var itemVM = inventory.ViewModel.Slots[invSlot.Value];
        if (itemVM == null) return;
        if (itemVM.Amount == 1)
        {
            viewModel.OfferItem((short)slot, invSlot.Value, 1);
        }
        else
        {
            _ownSlot = (short)slot;
            _inventorySlot = invSlot.Value;
            gameScreen.TradeAmountView.Open(_ownSlot, _inventorySlot);
        }
    }

    private void OnClosePressed(object? sender, MyraEventArgs e)
    {
        viewModel.Close();
        Close();
    }

    private void OnAcceptOfferPressed(object? sender, MyraEventArgs e)
    {
        ConfirmOfferButton.Visible = true;
        AcceptOfferButton.Visible = DeclineOfferButton.Visible = false;
        OfferDisabledPanel.Visible = false;
        viewModel.Accept();
    }

    private void OnDeclineOfferPressed(object? sender, MyraEventArgs e)
    {
        ConfirmOfferButton.Visible = true;
        AcceptOfferButton.Visible = DeclineOfferButton.Visible = false;
        OfferDisabledPanel.Visible = false;
        viewModel.Decline();
    }

    private void OnConfirmPressed(object? sender, MyraEventArgs e)
    {
        ConfirmOfferButton.Visible = AcceptOfferButton.Visible = DeclineOfferButton.Visible = false;
        OfferDisabledPanel.Visible = true;
        viewModel.Confirm();
    }
}
