using System.Windows.Forms;
using CryBits.Editors.AvaloniaUI.Forms;

namespace CryBits.Editors.AvaloniaUI;

internal static class AvaloniaTilesLauncher
{
    public static void OpenTilesEditor(Form owner)
    {
        owner.Hide();
        AvaloniaRuntime.RunOnUiThread(() =>
        {
            var window = new EditorTilesWindow();
            window.Closed += (_, _) => owner.BeginInvoke(owner.Show);
            window.Show();
        });
    }
}
