using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Network.Senders;
using CryBits.Client.Utils;

namespace CryBits.Client.UI.Game.Views;

internal class DropItemView(PlayerSender playerSender) : IView
{
    internal static Panel Panel => Tools.Panels["Drop"];
    internal static TextBox AmountTextBox => Tools.TextBoxes["Drop_Amount"];
    private static Button ConfirmButton => Tools.Buttons["Drop_Confirm"];
    private static Button CancelButton => Tools.Buttons["Drop_Cancel"];

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

        playerSender.DropItem(InventorySlot, amount);
        Panel.Visible = false;
    }

    private void OnCancelPressed()
    {
        Panel.Visible = false;
    }
}
