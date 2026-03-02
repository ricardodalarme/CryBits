using CryBits.Client.Entities;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Network.Senders;
using CryBits.Entities.Slots;
using CryBits.Enums;
using SFML.Window;
using static CryBits.Client.Utils.UIUtils;
using static CryBits.Globals;

namespace CryBits.Client.UI.Game.Views;

internal class TradeView(TradeSender tradeSender) : IView
{
    internal static Panel Panel => Tools.Panels["Trade"];
    internal static Panel OfferDisabledPanel => Tools.Panels["Trade_Offer_Disable"]; // TODO: add disable state to button instead
    private static Button CloseButton => Tools.Buttons["Trade_Close"];
    internal static Button AcceptOfferButton => Tools.Buttons["Trade_Offer_Accept"];
    internal static Button DeclineOfferButton => Tools.Buttons["Trade_Offer_Decline"];
    internal static Button ConfirmOfferButton => Tools.Buttons["Trade_Offer_Confirm"];

    public static short CurrentSlot => GetSlotAtMousePosition(Panel, 7, 50, 6, 5);
    public static short InventorySlot;

    public void Bind()
    {
        Panel.OnMouseDown += OnPanelMouseDown;
        Panel.OnMouseUp += OnPanelMouseUp;
        CloseButton.OnMouseUp += OnClosePressed;
        AcceptOfferButton.OnMouseUp += OnAcceptOfferPressed;
        DeclineOfferButton.OnMouseUp += OnDeclineOfferPressed;
        ConfirmOfferButton.OnMouseUp += OnConfirmOfferPressed;
    }

    public void Unbind()
    {
        Panel.OnMouseDown -= OnPanelMouseDown;
        Panel.OnMouseUp -= OnPanelMouseUp;
        CloseButton.OnMouseUp -= OnClosePressed;
        AcceptOfferButton.OnMouseUp -= OnAcceptOfferPressed;
        DeclineOfferButton.OnMouseUp -= OnDeclineOfferPressed;
        ConfirmOfferButton.OnMouseUp -= OnConfirmOfferPressed;
    }

    private void OnPanelMouseDown(MouseButtonEventArgs e)
    {
        var slot = CurrentSlot;

        if (!Panel.Visible) return;
        if (slot == -1) return;
        if (Player.Me.TradeOffer[slot].Item == null) return;

        if (e.Button == Mouse.Button.Right) tradeSender.TradeOffer(slot, 0);
    }

    private void OnPanelMouseUp()
    {
        if (GameScreen.InventoryChange <= 0) return;

        // Add item to trade
        if (Player.Me.Inventory[GameScreen.InventoryChange].Amount == 1)
            tradeSender.TradeOffer(CurrentSlot, GameScreen.InventoryChange);
        else
        {
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
