using CryBits.Client.Framework.Network;
using CryBits.Client.Network.Senders;
using Myra.Events;
using Myra.Graphics2D.UI;

namespace CryBits.Client.UI.Menu.Views;

internal class RegisterView(
    UiContext uiContext,
    AuthSender authSender,
    Connection connection,
    MenuScreen menuScreen) : ViewBase
{
    private Panel RegisterPanel => uiContext.Get<Panel>("Register");
    private TextBox UsernameTextBox => uiContext.Get<TextBox>("RegisterUsername");
    private TextBox PasswordTextBox => uiContext.Get<TextBox>("RegisterPassword");
    private TextBox PasswordConfirmTextBox => uiContext.Get<TextBox>("RegisterConfirm");

    private Button RegisterButton => uiContext.Get<Button>("RegisterConfirmBtn");
    private Button BackButton => uiContext.Get<Button>("RegisterBackBtn");

    public void Open()
    {
        RegisterPanel.Visible = true;
        UsernameTextBox.Text = string.Empty;
        PasswordTextBox.Text = string.Empty;
        PasswordConfirmTextBox.Text = string.Empty;
        Bind();
    }

    public void Close()
    {
        RegisterPanel.Visible = false;
        Unbind();
    }

    public override void Bind()
    {
        RegisterButton.Click += OnRegisterPressed;
        BackButton.Click += OnBackPressed;
    }

    public override void Unbind()
    {
        RegisterButton.Click -= OnRegisterPressed;
        BackButton.Click -= OnBackPressed;
    }

    private void OnRegisterPressed(object? sender, MyraEventArgs e)
    {
        if (!connection.IsConnected)
        {
            Dialog.CreateMessageBox("Error", "Can't connect to server!").ShowModal(uiContext.Desktop);
            return;
        }

        var username = UsernameTextBox.Text;
        var password = PasswordTextBox.Text;
        var confirmPassword = PasswordConfirmTextBox.Text;

        if (password != confirmPassword)
        {
            Dialog.CreateMessageBox("Error", "Passwords do not match!").ShowModal(uiContext.Desktop);
            return;
        }

        authSender.Register(username, password);
    }

    private void OnBackPressed(object? sender, MyraEventArgs e) => menuScreen.ShowLogin();
}
