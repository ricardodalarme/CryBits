using Avalonia.Controls;
using CryBits.Editors.Forms;

namespace CryBits.Editors.AvaloniaUI;

internal static class AvaloniaTilesLauncher
{
    public static void OpenTilesEditor(Window owner)
    {
        owner.Hide();
        var window = new EditorTilesWindow();
        window.Closed += (_, _) => owner.Show();
        window.Show();
    }
}
