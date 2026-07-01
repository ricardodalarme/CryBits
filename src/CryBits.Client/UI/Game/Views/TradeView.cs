using CryBits.Client.Framework.UI.Entities;
using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Network.Senders;
using CryBits.Client.Worlds;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Common;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Slots;
using CryBits.Simulation.Components;
using CryBits.Simulation.Intents;
using Iguina.Entities;
using System.Drawing;
using static CryBits.Definitions.Globals;

namespace CryBits.Client.UI.Game.Views;

internal class TradeView(IguinaContext uiContext, IntentSender intentSender, ItemRenderer itemRenderer, GameContext context, DefinitionCatalog catalog) : ViewBase
{
    internal Panel Panel => uiContext.Get<Panel>("Trade");
    internal Panel OfferDisabledPanel => uiContext.Get<Panel>("TradeOfferDisable");
    private Button CloseButton => uiContext.Get<Button>("TradeClose");
    internal Button AcceptOfferButton => uiContext.Get<Button>("TradeAccept");
    internal Button DeclineOfferButton => uiContext.Get<Button>("TradeDecline");
    internal Button ConfirmOfferButton => uiContext.Get<Button>("TradeConfirm");
    private SlotGrid OwnGrid => uiContext.Get<SlotGrid>("TradeGridOwn");
    private SlotGrid TheirGrid => uiContext.Get<SlotGrid>("TradeGridTheir");

    public static short OwnSlot;
    public static short InventorySlot;

    public override void Bind()
    {
        OwnGrid.OnSlotRightClick += OnOwnSlotRightClick;
        OwnGrid.OnSlotLeftUp += OnOwnSlotLeftUp;
        CloseButton.Events.OnClick += OnClosePressed;
        AcceptOfferButton.Events.OnClick += OnAcceptOfferPressed;
        DeclineOfferButton.Events.OnClick += OnDeclineOfferPressed;
        ConfirmOfferButton.Events.OnClick += OnConfirmOfferPressed;
        uiContext.PostDraw += OnPostDraw;
    }

    public override void Unbind()
    {
        OwnGrid.OnSlotRightClick -= OnOwnSlotRightClick;
        OwnGrid.OnSlotLeftUp -= OnOwnSlotLeftUp;
        CloseButton.Events.OnClick -= OnClosePressed;
        AcceptOfferButton.Events.OnClick -= OnAcceptOfferPressed;
        DeclineOfferButton.Events.OnClick -= OnDeclineOfferPressed;
        ConfirmOfferButton.Events.OnClick -= OnConfirmOfferPressed;
        uiContext.PostDraw -= OnPostDraw;
    }

    private void OnOwnSlotRightClick(int slot)
    {
        if (!Panel.Visible) return;

        var trade = context.LocalPlayer.GetTrade();
        var inv = context.LocalPlayer.GetInventory();
        if (trade?.Offer == null || inv == null) return;
        if (slot >= trade.Offer.Length) return;
        if (inv.Slots[trade.Offer[slot].SlotNum].ItemId == Guid.Empty) return;

        intentSender.Send(new TradeOfferIntent(default, (byte)slot, 0, 0));
    }

    private void OnOwnSlotLeftUp(int slot)
    {
        GameScreen.InventoryChange = null;
        var invSlot = InventoryView.DragOrigin;
        if (invSlot == null) return;

        var inv = context.LocalPlayer.GetInventory();
        if (inv == null) return;
        if (inv.Slots[invSlot.Value].Amount == 1)
            intentSender.Send(new TradeOfferIntent(default, (byte)slot, invSlot.Value, 1));
        else
        {
            OwnSlot = (short)slot;
            InventorySlot = invSlot.Value;
            GameScreen.Instance.TradeAmountView.AmountInput.Value = string.Empty;
            GameScreen.Instance.TradeAmountView.Panel.Visible = true;
        }
    }

    private void OnClosePressed(Entity _)
    {
        intentSender.Send(new TradeLeaveIntent(default));
        Panel.Visible = false;
    }

    private void OnAcceptOfferPressed(Entity _)
    {
        ConfirmOfferButton.Visible = true;
        AcceptOfferButton.Visible = DeclineOfferButton.Visible = false;
        OfferDisabledPanel.Visible = false;
        intentSender.Send(new TradeOfferStateIntent(default, TradeStatus.Accepted));

        var playerEntity = context.LocalPlayer.Entity;
        if (playerEntity != null)
        {
            var trade = context.World.Get<TradeState>(playerEntity.Value);
            if (trade != null)
                context.World.Set(playerEntity.Value, trade with { Offer = new TradeSlot[MaxInventory] });
        }
    }

    private void OnDeclineOfferPressed(Entity _)
    {
        ConfirmOfferButton.Visible = true;
        AcceptOfferButton.Visible = DeclineOfferButton.Visible = false;
        OfferDisabledPanel.Visible = false;
        intentSender.Send(new TradeOfferStateIntent(default, TradeStatus.Declined));
    }

    private void OnConfirmOfferPressed(Entity _)
    {
        ConfirmOfferButton.Visible = AcceptOfferButton.Visible = DeclineOfferButton.Visible = false;
        OfferDisabledPanel.Visible = true;
        intentSender.Send(new TradeOfferStateIntent(default, TradeStatus.Confirmed));
    }

    private void OnPostDraw()
    {
        if (!Panel.Visible) return;

        var trade = context.LocalPlayer.GetTrade();
        var inv = context.LocalPlayer.GetInventory();
        if (trade?.Offer == null || inv == null) return;

        for (var i = 0; i < OwnGrid.TotalSlots; i++)
        {
            if (i >= trade.Offer.Length) break;
            var rect = OwnGrid.GetSlotRect(i);
            var offer = trade.Offer[i];
            var inventorySlot = inv.Slots[offer.SlotNum];
            if (catalog.Items.Get(inventorySlot.ItemId) is { } item)
                itemRenderer.DrawItem(item, offer.Amount, new Point(rect.X, rect.Y));
        }

        if (trade.TheirOffer == null) return;
        for (var i = 0; i < TheirGrid.TotalSlots; i++)
        {
            if (i >= trade.TheirOffer.Length) break;
            var rect = TheirGrid.GetSlotRect(i);
            var offer = trade.TheirOffer[i];
            var inventorySlot = inv.Slots[offer.SlotNum];
            if (catalog.Items.Get(inventorySlot.ItemId) is { } item)
                itemRenderer.DrawItem(item, offer.Amount, new Point(rect.X, rect.Y));
        }
    }
}
