using System.Drawing;
using CryBits.Client.Entities;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Network.Senders;
using CryBits.Entities.Slots;
using CryBits.Enums;
using SFML.Window;
using static CryBits.Globals;

namespace CryBits.Client.UI.Game.Views;

internal class TradeView(TradeSender tradeSender, ItemRenderer itemRenderer) : IView
{
    internal static Panel Panel => Tools.Panels["Trade"];
    internal static Panel OfferDisabledPanel => Tools.Panels["Trade_Offer_Disable"]; // TODO: add disable state to button instead
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

    private void OnRenderOwnSlot(int slot, Point pos) =>
        itemRenderer.DrawItem(Player.Me.TradeOffer[slot].Item, Player.Me.TradeOffer[slot].Amount, pos);

    private void OnRenderTheirSlot(int slot, Point pos) =>
        itemRenderer.DrawItem(Player.Me.TradeTheirOffer[slot].Item, Player.Me.TradeTheirOffer[slot].Amount, pos);

    private void OnGridMouseDown(MouseButtonEventArgs e, short slot)
    {
        if (!Panel.Visible) return;
        if (Player.Me.TradeOffer[slot].Item == null) return;

        if (e.Button == Mouse.Button.Right) tradeSender.TradeOffer(slot, 0);
    }

    private void OnGridMouseUp(short slot)
    {
        if (GameScreen.InventoryChange <= 0) return;

        // Add item to trade
        if (Player.Me.Inventory[GameScreen.InventoryChange].Amount == 1)
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

        Player.Me.TradeOffer = new ItemSlot[MaxInventory];
        Player.Me.TradeTheirOffer = new ItemSlot[MaxInventory];
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
