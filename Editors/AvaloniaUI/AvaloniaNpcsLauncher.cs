using System.Windows.Forms;
using CryBits.Editors.AvaloniaUI.Forms;

namespace CryBits.Editors.AvaloniaUI;

internal static class AvaloniaNpcsLauncher
{
    public static void OpenNpcsEditor(Form owner)
    {
        owner.Hide();
        AvaloniaRuntime.RunOnUiThread(() =>
        {
            var window = new EditorNpcsWindow();
            window.Closed += (_, _) => owner.BeginInvoke(owner.Show);
            window.Show();
        });
    }
}
