using CryBits.Client.Framework.UI.Entities;
using CryBits.Client.Rendering.UI;
using CryBits.Client.UI.Game.ViewModels;
using CryBits.Definitions.Common;
using Iguina.Entities;
using System.Drawing;

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
    private SlotGrid OwnGrid => uiContext.Get<SlotGrid>("TradeGridOwn");
    private SlotGrid TheirGrid => uiContext.Get<SlotGrid>("TradeGridTheir");

    private short _ownSlot;
    private short _inventorySlot;

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
        OwnGrid.OnSlotRightClick += OnOwnSlotRightClick;
        OwnGrid.OnSlotLeftUp += OnOwnSlotLeftUp;
        CloseButton.Events.OnClick += OnClosePressed;
        AcceptOfferButton.Events.OnClick += OnAcceptOfferPressed;
        DeclineOfferButton.Events.OnClick += OnDeclineOfferPressed;
        ConfirmOfferButton.Events.OnClick += OnConfirmPressed;
        uiContext.PostDraw += OnPostDraw;
    }

    public override void Unbind()
    {
        OwnGrid.OnSlotRightClick -= OnOwnSlotRightClick;
        OwnGrid.OnSlotLeftUp -= OnOwnSlotLeftUp;
        CloseButton.Events.OnClick -= OnClosePressed;
        AcceptOfferButton.Events.OnClick -= OnAcceptOfferPressed;
        DeclineOfferButton.Events.OnClick -= OnDeclineOfferPressed;
        ConfirmOfferButton.Events.OnClick -= OnConfirmPressed;
        uiContext.PostDraw -= OnPostDraw;
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

        inventory.ViewModel.Refresh();
        var itemVM = inventory.ViewModel.Slots[invSlot.Value];
        if (itemVM == null) return;
        if (itemVM.Amount == 1)
            viewModel.OfferItem((short)slot, invSlot.Value, 1);
        else
        {
            _ownSlot = (short)slot;
            _inventorySlot = invSlot.Value;
            gameScreen.TradeAmountView.Open(_ownSlot, _inventorySlot);
        }
    }

    private void OnClosePressed(Entity _)
    {
        viewModel.Close();
        Close();
    }

    private void OnAcceptOfferPressed(Entity _)
    {
        ConfirmOfferButton.Visible = true;
        AcceptOfferButton.Visible = DeclineOfferButton.Visible = false;
        OfferDisabledPanel.Visible = false;
        viewModel.Accept();
    }

    private void OnDeclineOfferPressed(Entity _)
    {
        ConfirmOfferButton.Visible = true;
        AcceptOfferButton.Visible = DeclineOfferButton.Visible = false;
        OfferDisabledPanel.Visible = false;
        viewModel.Decline();
    }

    private void OnConfirmPressed(Entity _)
    {
        ConfirmOfferButton.Visible = AcceptOfferButton.Visible = DeclineOfferButton.Visible = false;
        OfferDisabledPanel.Visible = true;
        viewModel.Confirm();
    }

    private void OnPostDraw()
    {
        if (!Panel.Visible) return;

        var ownOffer = viewModel.OwnOffer;
        var theirOffer = viewModel.TheirOffer;

        for (var i = 0; i < OwnGrid.TotalSlots; i++)
        {
            if (i >= ownOffer.Length) break;
            var rect = OwnGrid.GetSlotRect(i);
            var offer = ownOffer[i];
            if (offer != null && offer.Definition is { } item)
                itemRenderer.DrawItem(item, offer.Amount, new Point(rect.X, rect.Y));
        }

        if (theirOffer == null) return;
        for (var i = 0; i < TheirGrid.TotalSlots; i++)
        {
            if (i >= theirOffer.Length) break;
            var rect = TheirGrid.GetSlotRect(i);
            var offer = theirOffer[i];
            if (offer != null && offer.Definition is { } item)
                itemRenderer.DrawItem(item, offer.Amount, new Point(rect.X, rect.Y));
        }
    }
}
