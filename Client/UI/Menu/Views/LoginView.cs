using CryBits.Client.Framework;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Framework.Persistence.Repositories;
using CryBits.Client.Network;
using CryBits.Client.Network.Senders;

namespace CryBits.Client.UI.Menu.Views;

internal class LoginView(NetworkClient networkClient, AuthSender authSender) : IView
{
    internal static Panel LoginPanel => Tools.Panels["Connect"];
    internal static TextBox UsernameTextBox => Tools.TextBoxes["Connect_Username"];
    private static TextBox PasswordTextBox => Tools.TextBoxes["Connect_Password"];
    internal static CheckBox SaveUsernameCheckBox => Tools.CheckBoxes["Connect_Save_Username"];
    private static Button ConfirmButton => Tools.Buttons["Connect_Confirm"];
    private static Button RegisterButton => Tools.Buttons["Register"];

    public void Bind()
    {
        SaveUsernameCheckBox.OnMouseUp += OnSaveUsernameChanged;
        ConfirmButton.OnMouseUp += OnConfirmPressed;
        RegisterButton.OnMouseUp += OnRegisterPressed;
    }

    public void Unbind()
    {
        SaveUsernameCheckBox.OnMouseUp -= OnSaveUsernameChanged;
        ConfirmButton.OnMouseUp -= OnConfirmPressed;
        RegisterButton.OnMouseUp -= OnRegisterPressed;
    }

    private void OnSaveUsernameChanged()
    {
        Options.SaveUsername = SaveUsernameCheckBox.Checked;
        OptionsRepository.Write();
    }

    private void OnConfirmPressed()
    {
        // Save username
        Options.Username = UsernameTextBox.Text;
        OptionsRepository.Write();

        // Connect to game
        if (networkClient.TryConnect()) authSender.Connect(UsernameTextBox.Text, PasswordTextBox.Text);
    }

    private void OnRegisterPressed()
    {
        networkClient.Disconnect();

        MenuScreen.CloseMenus();
        RegisterView.RegisterPanel.Visible = true;
    }
}
