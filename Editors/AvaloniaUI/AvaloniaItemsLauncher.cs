using Avalonia.Controls;
using CryBits.Editors.Forms;

namespace CryBits.Editors.AvaloniaUI;

internal static class AvaloniaItemsLauncher
{
    public static void OpenItemsEditor(Window owner)
    {
        owner.Hide();
        var window = new EditorItemsWindow();
        window.Closed += (_, _) => owner.Show();
        window.Show();
    }
}
