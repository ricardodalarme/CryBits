using CryBits.Client.Network.Senders;
using CryBits.Simulation.Intents;
using Myra.Events;
using Myra.Graphics2D.UI;

namespace CryBits.Client.UI.Game.Views;

internal class TradeAmountView(UiContext uiContext, IntentSender intentSender) : ViewBase
{
    private Panel Panel => uiContext.Get<Panel>("TradeAmount");
    private SpinButton AmountInput => uiContext.Get<SpinButton>("TradeAmountInput");
    private Button ConfirmButton => uiContext.Get<Button>("TradeAmtConfirm");
    private Button CancelButton => uiContext.Get<Button>("TradeAmtCancel");

    private short _ownSlot;
    private short _inventorySlot;

    public void Open(short ownSlot, short inventorySlot)
    {
        _ownSlot = ownSlot;
        _inventorySlot = inventorySlot;
        AmountInput.Value = 1;
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
        ConfirmButton.Click += OnConfirmPressed;
        CancelButton.Click += OnCancelPressed;
    }

    public override void Unbind()
    {
        ConfirmButton.Click -= OnConfirmPressed;
        CancelButton.Click -= OnCancelPressed;
    }

    private void OnConfirmPressed(object? sender, MyraEventArgs e)
    {
        var amount = AmountInput.Value ?? 0;
        if (amount <= 0)
        {
            Dialog.CreateMessageBox("Invalid", "Enter a valid value!").ShowModal(uiContext.Desktop);
            return;
        }

        intentSender.Send(new TradeOfferIntent(default, _ownSlot, _inventorySlot, (short)amount));
        Close();
    }

    private void OnCancelPressed(object? sender, MyraEventArgs e)
    {
        Close();
    }
}
