using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Framework.Network;
using CryBits.Client.Network.Senders;

namespace CryBits.Client.UI.Menu.Views;

internal class RegisterView(NetworkClient networkClient, AuthSender authSender) : IView
{
    internal static Panel RegisterPanel => Tools.Panels["Register"];
    private static TextBox UsernameTextBox => Tools.TextBoxes["Register_Username"];
    private static TextBox PasswordTextBox => Tools.TextBoxes["Register_Password"];
    private static TextBox ConfirmPasswordTextBox => Tools.TextBoxes["Register_Password2"];
    private static Button ConfirmButton => Tools.Buttons["Register_Confirm"];
    private static Button LoginButton => Tools.Buttons["Connect"];

    public void Bind()
    {
        ConfirmButton.OnMouseUp += OnConfirmPressed;
        LoginButton.OnMouseUp += OnLoginPressed;
    }

    public void Unbind()
    {
        ConfirmButton.OnMouseUp -= OnConfirmPressed;
        LoginButton.OnMouseUp -= OnLoginPressed;
    }

    private void OnConfirmPressed()
    {
        // Basic validation
        if (PasswordTextBox.Text != ConfirmPasswordTextBox.Text)
        {
            Alert.Show("The password don't match.");
            return;
        }

        if (!networkClient.TryConnect())
        {
            Alert.Show("The server is currently unavailable.");
            return;
        }

        authSender.Register(UsernameTextBox.Text, PasswordTextBox.Text);
    }

    private void OnLoginPressed()
    {
        networkClient.Disconnect();

        MenuScreen.CloseMenus();
        LoginView.LoginPanel.Visible = true;
    }
}
