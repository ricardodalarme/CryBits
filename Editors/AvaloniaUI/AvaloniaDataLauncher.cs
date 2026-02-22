using Avalonia.Controls;
using CryBits.Editors.Forms;

namespace CryBits.Editors.AvaloniaUI;

internal static class AvaloniaDataLauncher
{
    public static void OpenDataEditor(Window owner)
    {
        owner.Hide();
        var window = new EditorDataWindow();
        window.Closed += (_, _) => owner.Show();
        window.Show();
    }
}
