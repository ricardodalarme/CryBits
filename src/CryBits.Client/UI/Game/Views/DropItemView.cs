using CryBits.Client.Network.Senders;
using CryBits.Simulation.Intents;
using Myra.Events;
using Myra.Graphics2D.UI;

namespace CryBits.Client.UI.Game.Views;

internal class DropItemView(UiContext uiContext, IntentSender intentSender) : ViewBase
{
    internal Panel Panel => uiContext.Get<Panel>("Drop");
    internal SpinButton AmountInput => uiContext.Get<SpinButton>("DropAmount");
    private Button ConfirmButton => uiContext.Get<Button>("DropConfirm");
    private Button CancelButton => uiContext.Get<Button>("DropCancel");

    private short _inventorySlot;

    public void Show(short slot)
    {
        _inventorySlot = slot;
        Panel.Visible = true;
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

        intentSender.Send(new DropItemIntent(default, (byte)_inventorySlot, (short)amount));
        Panel.Visible = false;
    }

    private void OnCancelPressed(object? sender, MyraEventArgs e)
    {
        Panel.Visible = false;
    }
}
