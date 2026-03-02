using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Network.Senders;
using CryBits.Client.Utils;

namespace CryBits.Client.UI.Game.Views;

internal class TradeAmountView(TradeSender tradeSender) : IView
{
    internal static Panel Panel => Tools.Panels["Trade_Amount"];
    internal static TextBox AmountTextBox => Tools.TextBoxes["Trade_Amount"];
    private static Button ConfirmButton => Tools.Buttons["Trade_Amount_Confirm"];
    private static Button CancelButton => Tools.Buttons["Trade_Amount_Cancel"];

    public void Bind()
    {
        ConfirmButton.OnMouseUp += OnConfirmPressed;
        CancelButton.OnMouseUp += OnCancelPressed;
    }

    public void Unbind()
    {
        ConfirmButton.OnMouseUp -= OnConfirmPressed;
        CancelButton.OnMouseUp -= OnCancelPressed;
    }

    private void OnConfirmPressed()
    {
        // Validate entered amount
        if (!short.TryParse(AmountTextBox.Text, out var amount) || amount <= 0)
        {
            Alert.Show("Enter a valid value!");
            return;
        }

        tradeSender.TradeOffer(TradeView.CurrentSlot, TradeView.InventorySlot, amount);
        Panel.Visible = false;
    }

    private void OnCancelPressed()
    {
        Panel.Visible = false;
    }
}
