using CryBits.Client.Network.Senders;
using CryBits.Simulation.Intents;
using Iguina.Entities;

namespace CryBits.Client.UI.Game.Views;

internal class DropItemView(UiContext uiContext, IntentSender intentSender) : ViewBase
{
    internal Panel Panel => uiContext.Get<Panel>("Drop");
    internal NumericInput AmountInput => uiContext.Get<NumericInput>("DropAmount");
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

        intentSender.Send(new DropItemIntent(default, (byte)_inventorySlot, (short)AmountInput.NumericValue));
        Panel.Visible = false;
    }

    private void OnCancelPressed(Entity _)
    {
        Panel.Visible = false;
    }
}
