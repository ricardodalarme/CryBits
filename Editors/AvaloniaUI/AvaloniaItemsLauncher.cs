using System.Windows.Forms;
using CryBits.Editors.AvaloniaUI.Forms;

namespace CryBits.Editors.AvaloniaUI;

internal static class AvaloniaItemsLauncher
{
    public static void OpenItemsEditor(Form owner)
    {
        owner.Hide();
        AvaloniaRuntime.RunOnUiThread(() =>
        {
            var window = new EditorItemsWindow();
            window.Closed += (_, _) => owner.BeginInvoke(owner.Show);
            window.Show();
        });
    }
}
