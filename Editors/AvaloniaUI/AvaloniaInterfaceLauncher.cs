using Avalonia.Controls;
using CryBits.Editors.AvaloniaUI.Forms;

namespace CryBits.Editors.AvaloniaUI;

internal static class AvaloniaInterfaceLauncher
{
    public static void OpenInterfaceEditor(Window owner)
    {
        owner.Hide();
        var window = new EditorInterfaceWindow();
        window.Closed += (_, _) => owner.Show();
        window.Show();
    }
}
