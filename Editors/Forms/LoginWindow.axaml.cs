using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CryBits.Client.Framework;
using CryBits.Editors.AvaloniaUI;
using CryBits.Editors.Network;

namespace CryBits.Editors.Forms;

internal partial class LoginWindow : Window
{
    private static LoginWindow? _instance;

    /// <summary>The entered username (persisted across connection attempts).</summary>
    public static string Username { get; set; } = string.Empty;

    /// <summary>The entered password (persisted across connection attempts).</summary>
    public static string Password { get; set; } = string.Empty;

    /// <summary>Shows the login window (singleton), creating it if necessary.</summary>
    public static void Open()
    {
        AvaloniaRuntime.RunOnUiThread(() =>
        {
            if (_instance == null)
            {
                _instance = new LoginWindow();
                _instance.Closed += (_, _) =>
                {
                    _instance = null;
                    Program.Working = false;
                };
            }

            if (!_instance.IsVisible)
                _instance.Show();

            _instance.Activate();
        });
    }

    /// <summary>Hides the login window if visible.</summary>
    public static void HideWindow()
    {
        AvaloniaRuntime.RunOnUiThread(() =>
        {
            if (_instance != null && _instance.IsVisible)
                _instance.Hide();
        });
    }

    public LoginWindow()
    {
        InitializeComponent();
        txtUsername.Text = Options.Username;
        chkUsername.IsChecked = Options.Username != string.Empty;
    }

    private void butConnect_Click(object sender, RoutedEventArgs e)
    {
        Username = txtUsername.Text ?? string.Empty;
        Password = txtPassword.Text ?? string.Empty;

        if (!Socket.TryConnect())
        {
            MessageBox.Show(@"The server is currently unavailable.");
            return;
        }

        if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
        {
            MessageBox.Show(@"Some field is empty.");
            return;
        }

        Send.Connect();

        Options.Username = chkUsername.IsChecked == true ? Username : string.Empty;
        Client.Framework.Persistence.Repositories.OptionsRepository.Write();
    }
}
