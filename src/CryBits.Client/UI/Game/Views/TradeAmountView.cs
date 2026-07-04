using CryBits.Client.Network.Senders;
using CryBits.Simulation.Intents;
using Iguina.Entities;

namespace CryBits.Client.UI.Game.Views;

internal class TradeAmountView(UiContext uiContext, IntentSender intentSender) : ViewBase
{
    private Panel Panel => uiContext.Get<Panel>("TradeAmount");
    private NumericInput AmountInput => uiContext.Get<NumericInput>("TradeAmountInput");
    private Button ConfirmButton => uiContext.Get<Button>("TradeAmtConfirm");
    private Button CancelButton => uiContext.Get<Button>("TradeAmtCancel");

    private short _ownSlot;
    private short _inventorySlot;

    public void Open(short ownSlot, short inventorySlot)
    {
        _ownSlot = ownSlot;
        _inventorySlot = inventorySlot;
        AmountInput.Value = string.Empty;
        Panel.Visible = true;
        Bind();
    }

    public void Close()
    {
        Panel.Visible = false;
        Unbind();
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
        Close();
    }

    private void OnCancelPressed(Entity _)
    {
        Close();
    }
}
