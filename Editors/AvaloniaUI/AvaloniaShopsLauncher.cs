using Avalonia.Controls;
using CryBits.Editors.AvaloniaUI.Forms;
using CryBits.Entities;

namespace CryBits.Editors.AvaloniaUI;

internal static class AvaloniaShopsLauncher
{
    public static void OpenShopsEditor(Window owner)
    {
        if (Item.List.Count == 0)
        {
            MessageBox.Show(@"It must have at least one item registered to open the store editor.");
            return;
        }

        owner.Hide();
        var window = new EditorShopsWindow();
        window.Closed += (_, _) => owner.Show();
        window.Show();
    }
}
