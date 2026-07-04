using CryBits.Client.Framework;
using CryBits.Client.Framework.Network;
using CryBits.Client.Framework.Persistence.Repositories;
using CryBits.Client.Network.Senders;
using Iguina.Entities;

namespace CryBits.Client.UI.Menu.Views;

internal class LoginView(UiContext uiContext, AuthSender authSender, Connection connection, MenuScreen menuScreen) : ViewBase
{
    private Panel LoginPanel => uiContext.Get<Panel>("Login");
    private TextInput UsernameTextBox => uiContext.Get<TextInput>("Username");
    private TextInput PasswordTextBox => uiContext.Get<TextInput>("Password");
    private Checkbox SaveUsernameCheckbox => uiContext.Get<Checkbox>("SaveUsername");
    private Button ConfirmButton => uiContext.Get<Button>("LoginConfirm");
    private Button RegisterButton => uiContext.Get<Button>("LoginRegister");

    public void Open()
    {
        SaveUsernameCheckbox.Checked = Options.Instance.SaveUsername;
        if (Options.Instance.SaveUsername)
            UsernameTextBox.Value = Options.Instance.Username;
        else
            UsernameTextBox.Value = string.Empty;

        PasswordTextBox.Value = string.Empty;
        LoginPanel.Visible = true;
        Bind();
    }

    public void Close()
    {
        LoginPanel.Visible = false;
        Unbind();
    }

    public override void Bind()
    {
        SaveUsernameCheckbox.Events.OnValueChanged += OnSaveUsernameChanged;
        ConfirmButton.Events.OnClick += OnConfirmPressed;
        RegisterButton.Events.OnClick += OnRegisterPressed;
    }

    public override void Unbind()
    {
        SaveUsernameCheckbox.Events.OnValueChanged -= OnSaveUsernameChanged;
        ConfirmButton.Events.OnClick -= OnConfirmPressed;
        RegisterButton.Events.OnClick -= OnRegisterPressed;
    }

    private void OnSaveUsernameChanged(Entity _)
    {
        Options.Instance.SaveUsername = SaveUsernameCheckbox.Checked;
        OptionsRepository.Write();
    }

    private void OnConfirmPressed(Entity _)
    {
        Options.Instance.Username = UsernameTextBox.Value;
        OptionsRepository.Write();

        if (!connection.TryConnect())
        {
            uiContext.UISystem?.MessageBoxes.ShowInfoMessageBox("Server", "The server is currently unavailable.");
            return;
        }

        authSender.Connect(UsernameTextBox.Value, PasswordTextBox.Value);
    }

    private void OnRegisterPressed(Entity _)
    {
        connection.Disconnect();
        menuScreen.ShowRegister();
    }
}
