using Avalonia.Threading;
using CryBits.Editors.Forms;

namespace CryBits.Editors.AvaloniaUI;

internal static class AvaloniaMapsLauncher
{
    /// <summary>
    /// Opens the Maps editor. Because Maps is the primary owner window
    /// (launched directly from Program.cs) there is no parent to hide — we
    /// just construct and show it directly.
    /// </summary>
    public static void OpenMapsEditor()
    {
        Dispatcher.UIThread.Post(() =>
        {
            var window = new EditorMapsWindow();
            window.Show();
        });
    }

    /// <summary>
    /// Opens the Maps editor from another parent window, hiding the parent
    /// while Maps is open (same pattern as other editor launchers).
    /// </summary>
    public static void OpenMapsEditor(Avalonia.Controls.Window parent)
    {
        Dispatcher.UIThread.Post(async () =>
        {
            parent.Hide();
            var window = new EditorMapsWindow();
            await window.ShowDialog(parent);
            parent.Show();
        });
    }
}
