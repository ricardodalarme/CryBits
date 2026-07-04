using CryBits.Client.Network.Senders;
using CryBits.Simulation.Intents;
using Iguina.Entities;

namespace CryBits.Client.UI.Game.Views;

internal class TradeAmountView(UiContext uiContext, IntentSender intentSender) : ViewBase
{
    internal Panel Panel => uiContext.Get<Panel>("TradeAmount");
    internal NumericInput AmountInput => uiContext.Get<NumericInput>("TradeAmountInput");
    private Button ConfirmButton => uiContext.Get<Button>("TradeAmtConfirm");
    private Button CancelButton => uiContext.Get<Button>("TradeAmtCancel");

    private short _ownSlot;
    private short _inventorySlot;

    public void Show(short ownSlot, short inventorySlot)
    {
        _ownSlot = ownSlot;
        _inventorySlot = inventorySlot;
        Panel.Visible = true;
    }

    public override void Bind()
    {
        ConfirmButton.Events.OnClick += OnConfirmPressed;
        CancelButton.Events.OnClick += OnCancelPressed;
    }

    public override void Unbind()
    {
        ConfirmButton.Events.OnClick -= OnConfirmPressed;
        CancelButton.Events.OnClick -= OnCancelPressed;
    }

    private void OnConfirmPressed(Entity _)
    {
        if (AmountInput.NumericValue <= 0)
        {
            uiContext.UISystem?.MessageBoxes.ShowInfoMessageBox("Invalid", "Enter a valid value!");
            return;
        }

        intentSender.Send(new TradeOfferIntent(default, _ownSlot, _inventorySlot, (short)AmountInput.NumericValue));
        Panel.Visible = false;
    }

    private void OnCancelPressed(Entity _)
    {
        Panel.Visible = false;
    }
}
