using CryBits.Client.Framework.Network;
using CryBits.Client.Network.Senders;
using Iguina.Entities;

namespace CryBits.Client.UI.Menu.Views;

internal class RegisterView(IguinaContext uiContext, AuthSender authSender) : ViewBase
{
    internal Panel RegisterPanel => uiContext.Get<Panel>("Register");
    private TextInput UsernameTextBox => uiContext.Get<TextInput>("RegisterUsername");
    private TextInput PasswordTextBox => uiContext.Get<TextInput>("RegisterPassword");
    private TextInput ConfirmPasswordTextBox => uiContext.Get<TextInput>("RegisterConfirm");
    private Button ConfirmButton => uiContext.Get<Button>("RegisterConfirmBtn");
    private Button LoginButton => uiContext.Get<Button>("RegisterBackBtn");

    public override void Bind()
    {
        ConfirmButton.Events.OnClick += OnConfirmPressed;
        LoginButton.Events.OnClick += OnLoginPressed;
    }

    public override void Unbind()
    {
        ConfirmButton.Events.OnClick -= OnConfirmPressed;
        LoginButton.Events.OnClick -= OnLoginPressed;
    }

    private void OnConfirmPressed(Entity _)
    {
        if (PasswordTextBox.Value != ConfirmPasswordTextBox.Value)
        {
            uiContext.UISystem?.MessageBoxes.ShowInfoMessageBox("Registration", "The passwords don't match.");
            return;
        }

        if (!Connection.Instance.TryConnect())
        {
            uiContext.UISystem?.MessageBoxes.ShowInfoMessageBox("Server", "The server is currently unavailable.");
            return;
        }

        authSender.Register(UsernameTextBox.Value, PasswordTextBox.Value);
    }

    private void OnLoginPressed(Entity _)
    {
        Connection.Instance.Disconnect();

        MenuScreen.Instance.CloseMenus();
        MenuScreen.Instance.LoginView.LoginPanel.Visible = true;
    }
}
