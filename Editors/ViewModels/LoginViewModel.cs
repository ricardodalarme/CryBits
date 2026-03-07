using CryBits.Client.Framework;
using CryBits.Client.Framework.Persistence.Repositories;
using CryBits.Editors.Network;

namespace CryBits.Editors.ViewModels;

internal sealed class LoginViewModel : ViewModelBase
{
    private string _username;
    private string _password = string.Empty;
    private bool _saveUsername;

    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public bool SaveUsername
    {
        get => _saveUsername;
        set => SetProperty(ref _saveUsername, value);
    }

    public LoginViewModel()
    {
        _username = Options.Username;
        _saveUsername = Options.Username != string.Empty;
    }

    /// <summary>
    /// Attempts to connect to the server using the current credentials.
    /// Returns null on success, or an error message on failure.
    /// </summary>
    public string? TryConnect()
    {
        if (!NetworkClient.TryConnect())
            return "The server is currently unavailable.";

        if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            return "Some field is empty.";

        PackageSender.Connect(Username, Password);

        Options.Username = SaveUsername ? Username : string.Empty;
        OptionsRepository.Write();

        return null;
    }
}
