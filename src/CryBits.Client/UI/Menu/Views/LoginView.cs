using CryBits.Client.Framework;
using CryBits.Client.Framework.Network;
using CryBits.Client.Network.Senders;
using Myra.Events;
using Myra.Graphics2D.UI;

namespace CryBits.Client.UI.Menu.Views;

internal class LoginView(
    UiContext uiContext,
    AuthSender authSender,
    Connection connection,
    MenuScreen menuScreen) : ViewBase
{
    private Panel LoginPanel => uiContext.Get<Panel>("Login");
    private TextBox UsernameTextBox => uiContext.Get<TextBox>("Username");
    private TextBox PasswordTextBox => uiContext.Get<TextBox>("Password");
    private CheckButton SaveAccountCheckbox => uiContext.Get<CheckButton>("SaveUsername");

    private Button LoginButton => uiContext.Get<Button>("LoginConfirm");
    private Button RegisterButton => uiContext.Get<Button>("LoginRegister");
    private Button OptionsButton => uiContext.Get<Button>("OptionsButton");

    public void Open()
    {
        LoginPanel.Visible = true;
        UsernameTextBox.Text = Options.Instance.Username;
        PasswordTextBox.Text = string.Empty;
        SaveAccountCheckbox.IsChecked = Options.Instance.SaveUsername;
        Bind();
    }

    public void Close()
    {
        LoginPanel.Visible = false;
        Unbind();
    }

    public override void Bind()
    {
        LoginButton.Click += OnLoginPressed;
        RegisterButton.Click += OnRegisterPressed;
        OptionsButton.Click += OnOptionsPressed;
    }

    public override void Unbind()
    {
        LoginButton.Click -= OnLoginPressed;
        RegisterButton.Click -= OnRegisterPressed;
        OptionsButton.Click -= OnOptionsPressed;
    }

    private void OnLoginPressed(object? sender, MyraEventArgs e)
    {
        if (!connection.IsConnected)
        {
            Dialog.CreateMessageBox("Error", "Can't connect to server!").ShowModal(uiContext.Desktop);
            return;
        }

        var username = UsernameTextBox.Text;
        var password = PasswordTextBox.Text;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            Dialog.CreateMessageBox("Invalid Credentials", "Username and password cannot be empty.").ShowModal(uiContext.Desktop);
            return;
        }

        authSender.Connect(username, password);

        Options.Instance.Username = SaveAccountCheckbox.IsChecked ? username : string.Empty;
        Options.Instance.SaveUsername = SaveAccountCheckbox.IsChecked;
    }

    private void OnRegisterPressed(object? sender, MyraEventArgs e) => menuScreen.ShowRegister();
    private void OnOptionsPressed(object? sender, MyraEventArgs e) => menuScreen.ShowOptions();
}
