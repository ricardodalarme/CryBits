using Avalonia.Controls;
using Avalonia.Threading;
using CryBits.Client.Framework.Network;
using CryBits.Editors.Core;
using CryBits.Editors.Network;

namespace CryBits.Editors.Forms.Login;

internal partial class LoginWindow : Window
{
    public static void Open(EditorShell shell)
    {
        Dispatcher.UIThread.Post(() =>
        {
            var window = shell.LoginWindow ??= new LoginWindow(shell.Sender, shell.Connection);
            window.Closed += (_, _) =>
            {
                if (shell.LoginWindow == window)
                    shell.LoginWindow = null;
                shell.Working = false;
            };

            if (!window.IsVisible)
                window.Show();

            window.Activate();
        });
    }

    public static void HideWindow(EditorShell shell)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (shell.LoginWindow != null && shell.LoginWindow.IsVisible)
                shell.LoginWindow.Hide();
        });
    }

    public LoginWindow(PackageSender sender, Connection connection)
    {
        InitializeComponent();
        DataContext = new LoginViewModel(connection, sender);
    }
}
