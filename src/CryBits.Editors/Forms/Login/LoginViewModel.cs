using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CryBits.Client.Framework;
using CryBits.Client.Framework.Network;
using CryBits.Editors.AvaloniaUI;
using CryBits.Editors.Network;

namespace CryBits.Editors.Forms.Login;

internal sealed partial class LoginViewModel : ObservableObject
{
    [ObservableProperty]
    private string _username = Options.Instance.Username;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private bool _saveUsername = Options.Instance.Username != string.Empty;

    [RelayCommand]
    private void Connect()
    {
        if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
        {
            MessageBox.Show("Some field is empty.");
            return;
        }

        if (Connection.Instance == null || !Connection.Instance.TryConnect())
        {
            MessageBox.Show("The server is currently unavailable.");
            return;
        }

        PackageSender.Instance.Connect(Username, Password);

        if (SaveUsername)
        {
            Options.Instance.Username = Username;
            Client.Framework.Persistence.Repositories.OptionsRepository.Write();
        }
        else
        {
            Options.Instance.Username = string.Empty;
            Client.Framework.Persistence.Repositories.OptionsRepository.Write();
        }
    }
}
