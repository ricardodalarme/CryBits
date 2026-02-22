using Avalonia.Controls;
using CryBits.Editors.Forms;

namespace CryBits.Editors.AvaloniaUI;

internal static class AvaloniaNpcsLauncher
{
    public static void OpenNpcsEditor(Window owner)
    {
        owner.Hide();
        var window = new EditorNpcsWindow();
        window.Closed += (_, _) => owner.Show();
        window.Show();
    }
}
