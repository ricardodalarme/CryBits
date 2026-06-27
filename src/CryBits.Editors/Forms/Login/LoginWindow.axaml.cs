using Avalonia.Controls;
using Avalonia.Threading;

namespace CryBits.Editors.Forms.Login;

internal partial class LoginWindow : Window
{
    private static LoginWindow? _instance;

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
        InitializeComponent();
        DataContext = new LoginViewModel();
    }
}
