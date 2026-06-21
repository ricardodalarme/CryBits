using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Network.Senders;
using CryBits.Client.Worlds;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Common;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Slots;
using SFML.Window;
using System.Drawing;
using static CryBits.Definitions.Globals;

namespace CryBits.Client.UI.Game.Views;

internal class TradeView(TradeSender tradeSender, ItemRenderer itemRenderer, GameContext context, DefinitionCatalog catalog) : IView
{
    private readonly DefinitionCatalog _catalog = catalog;
    internal static Panel Panel => Tools.Panels["Trade"];
    internal static Panel OfferDisabledPanel => Tools.Panels["Trade_Offer_Disable"];
    private static Button CloseButton => Tools.Buttons["Trade_Close"];
    internal static Button AcceptOfferButton => Tools.Buttons["Trade_Offer_Accept"];
    internal static Button DeclineOfferButton => Tools.Buttons["Trade_Offer_Decline"];
    internal static Button ConfirmOfferButton => Tools.Buttons["Trade_Offer_Confirm"];
    private static SlotGrid OwnGrid => Tools.SlotGrids["Trade_Grid_Own"];
    private static SlotGrid TheirGrid => Tools.SlotGrids["Trade_Grid_Their"];

    public static short OwnSlot;
    public static short InventorySlot;

    public void Bind()
    {
        OwnGrid.OnRenderSlot += OnRenderOwnSlot;
        TheirGrid.OnRenderSlot += OnRenderTheirSlot;
        OwnGrid.OnMouseDown += OnGridMouseDown;
        OwnGrid.OnMouseUp += OnGridMouseUp;
        CloseButton.OnMouseUp += OnClosePressed;
        AcceptOfferButton.OnMouseUp += OnAcceptOfferPressed;
        DeclineOfferButton.OnMouseUp += OnDeclineOfferPressed;
        ConfirmOfferButton.OnMouseUp += OnConfirmOfferPressed;
    }

    public void Unbind()
    {
        OwnGrid.OnRenderSlot -= OnRenderOwnSlot;
        TheirGrid.OnRenderSlot -= OnRenderTheirSlot;
        OwnGrid.OnMouseDown -= OnGridMouseDown;
        OwnGrid.OnMouseUp -= OnGridMouseUp;
        CloseButton.OnMouseUp -= OnClosePressed;
        AcceptOfferButton.OnMouseUp -= OnAcceptOfferPressed;
        DeclineOfferButton.OnMouseUp -= OnDeclineOfferPressed;
        ConfirmOfferButton.OnMouseUp -= OnConfirmOfferPressed;
    }

    private void OnRenderOwnSlot(int slot, Point pos)
    {
        var trade = context.LocalPlayer.GetTrade();
        var inv = context.LocalPlayer.GetInventory();
        if (trade?.Offer == null || inv == null) return;
        if (slot >= trade.Offer.Length) return;
        var offer = trade.Offer[slot];
        var inventorySlot = inv.Slots[offer.SlotNum];
        if (_catalog.Items.Get(inventorySlot.ItemId) is { } item)
            itemRenderer.DrawItem(item, offer.Amount, pos);
    }

    private void OnRenderTheirSlot(int slot, Point pos)
    {
        var trade = context.LocalPlayer.GetTrade();
        if (trade?.Offer == null) return;
        if (slot >= trade.Offer.Length) return;
        var offer = trade.Offer[slot];
        var item = _catalog.Items.Get(Guid.Empty);
        if (item != null)
            itemRenderer.DrawItem(item, offer.Amount, pos);
    }

    private void OnGridMouseDown(MouseButtonEventArgs e, short slot)
    {
        if (!Panel.Visible) return;
        var trade = context.LocalPlayer.GetTrade();
        var inv = context.LocalPlayer.GetInventory();
        if (trade?.Offer == null || inv == null) return;
        if (slot >= trade.Offer.Length) return;
        if (inv.Slots[trade.Offer[slot].SlotNum].ItemId == Guid.Empty) return;

        if (e.Button == Mouse.Button.Right) tradeSender.TradeOffer(slot, 0);
    }

    private void OnGridMouseUp(short slot)
    {
        if (GameScreen.InventoryChange <= 0) return;

        var inv = context.LocalPlayer.GetInventory();
        if (inv == null) return;
        if (inv.Slots[GameScreen.InventoryChange].Amount == 1)
            tradeSender.TradeOffer(slot, GameScreen.InventoryChange);
        else
        {
            OwnSlot = slot;
            InventorySlot = GameScreen.InventoryChange;
            TradeAmountView.AmountTextBox.Text = string.Empty;
            TradeAmountView.Panel.Visible = true;
        }
    }

    private void OnClosePressed()
    {
        tradeSender.TradeLeave();
        Panel.Visible = false;
    }

    private void OnAcceptOfferPressed()
    {
        ConfirmOfferButton.Visible = true;
        AcceptOfferButton.Visible = DeclineOfferButton.Visible = false;
        OfferDisabledPanel.Visible = false;
        tradeSender.TradeOfferState(TradeStatus.Accepted);

        var trade = context.LocalPlayer.GetTrade();
        if (trade != null)
        {
            trade.Offer = new TradeSlot[MaxInventory];
        }
    }

    private void OnDeclineOfferPressed()
    {
        ConfirmOfferButton.Visible = true;
        AcceptOfferButton.Visible = DeclineOfferButton.Visible = false;
        OfferDisabledPanel.Visible = false;
        tradeSender.TradeOfferState(TradeStatus.Declined);
    }

    private void OnConfirmOfferPressed()
    {
        ConfirmOfferButton.Visible = AcceptOfferButton.Visible = DeclineOfferButton.Visible = false;
        OfferDisabledPanel.Visible = true;
        tradeSender.TradeOfferState(TradeStatus.Confirmed);
    }
}
