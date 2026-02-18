using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CryBits.Client.Framework;
using CryBits.Editors.Network;

namespace CryBits.Editors.AvaloniaUI.Forms;

internal partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
        txtUsername.Text = Options.Username;
        chkUsername.IsChecked = Options.Username != string.Empty;
        Closed += Login_Closed;
    }

    private void butConnect_Click(object sender, RoutedEventArgs e)
    {
        AvaloniaLoginLauncher.Username = txtUsername.Text ?? string.Empty;
        AvaloniaLoginLauncher.Password = txtPassword.Text ?? string.Empty;

        if (!Socket.TryConnect())
        {
            MessageBox.Show(@"The server is currently unavailable.");
            return;
        }

        if (string.IsNullOrEmpty(AvaloniaLoginLauncher.Username) || string.IsNullOrEmpty(AvaloniaLoginLauncher.Password))
        {
            MessageBox.Show(@"Some field is empty.");
            return;
        }

        Send.Connect();

        Options.Username = chkUsername.IsChecked == true ? AvaloniaLoginLauncher.Username : string.Empty;
        Client.Framework.Library.Repositories.OptionsRepository.Write();
    }

    private void Login_Closed(object sender, EventArgs e)
    {
        Program.Working = false;
    }
}
