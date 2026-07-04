using CryBits.Client.UI.Game.ViewModels;
using Iguina.Entities;

namespace CryBits.Client.UI.Game.Views;

internal class ShopSellView(
    UiContext uiContext,
    ShopViewModel viewModel) : ViewBase
{
    internal Panel Panel => uiContext.Get<Panel>("ShopSell");
    internal NumericInput AmountInput => uiContext.Get<NumericInput>("SellAmount");
    private Button ConfirmButton => uiContext.Get<Button>("SellConfirm");
    private Button CancelButton => uiContext.Get<Button>("SellCancel");

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

        viewModel.Sell(_inventorySlot, (short)AmountInput.NumericValue);
        Panel.Visible = false;
    }

    private void OnCancelPressed(Entity _)
    {
        Panel.Visible = false;
    }
}
