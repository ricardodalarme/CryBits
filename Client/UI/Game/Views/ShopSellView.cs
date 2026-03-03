using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Network.Senders;

namespace CryBits.Client.UI.Game.Views;

internal class ShopSellView(ShopSender shopSender) : IView
{
    internal static Panel Panel => Tools.Panels["Shop_Sell"];
    internal static TextBox AmountTextBox => Tools.TextBoxes["Shop_Sell_Amount"];
    private static Button ConfirmButton => Tools.Buttons["Shop_Sell_Confirm"];
    private static Button CancelButton => Tools.Buttons["Shop_Sell_Cancel"];

    public static short InventorySlot;

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

        shopSender.ShopSell(InventorySlot, amount);
        Panel.Visible = false;
    }

    private void OnCancelPressed()
    {
        Panel.Visible = false;
    }
}
