using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using CryBits.Editors.AvaloniaUI;
using CryBits.Editors.ViewModels;

namespace CryBits.Editors.Forms;

internal partial class LoginWindow : Window
{
    private static LoginWindow? _instance;
    private readonly LoginViewModel _vm;

    /// <summary>Shows the login window (singleton), creating it if necessary.</summary>
    public static void Open()
    {
        Dispatcher.UIThread.Post(() =>
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
        Dispatcher.UIThread.Post(() =>
        {
            if (_instance != null && _instance.IsVisible)
                _instance.Hide();
        });
    }

    public LoginWindow()
    {
        _vm = new LoginViewModel();
        DataContext = _vm;
        InitializeComponent();
        txtUsername.Text = _vm.Username;
        chkUsername.IsChecked = _vm.SaveUsername;
    }

    private void butConnect_Click(object sender, RoutedEventArgs e)
    {
        _vm.Username = txtUsername.Text ?? string.Empty;
        _vm.Password = txtPassword.Text ?? string.Empty;
        _vm.SaveUsername = chkUsername.IsChecked == true;

        var error = _vm.TryConnect();
        if (error != null)
            MessageBox.Show(error);
    }
}
