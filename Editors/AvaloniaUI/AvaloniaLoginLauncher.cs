using System;
using CryBits.Editors.AvaloniaUI.Forms;

namespace CryBits.Editors.AvaloniaUI;

internal static class AvaloniaLoginLauncher
{
    private static LoginWindow _window;

    public static string Username { get; set; } = string.Empty;
    public static string Password { get; set; } = string.Empty;

    public static void ShowLogin()
    {
        AvaloniaRuntime.RunOnUiThread(() =>
        {
            if (_window == null)
            {
                _window = new LoginWindow();
                _window.Closed += (_, _) =>
                {
                    _window = null;
                    Program.Working = false;
                };
            }

            if (!_window.IsVisible)
                _window.Show();

            _window.Activate();
        });
    }

    public static void HideLogin()
    {
        AvaloniaRuntime.RunOnUiThread(() =>
        {
            if (_window != null && _window.IsVisible)
                _window.Hide();
        });
    }
}
