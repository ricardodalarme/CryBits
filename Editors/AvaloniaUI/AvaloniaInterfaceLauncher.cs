using System.Windows.Forms;
using CryBits.Editors.AvaloniaUI.Forms;

namespace CryBits.Editors.AvaloniaUI;

internal static class AvaloniaInterfaceLauncher
{
    public static void OpenInterfaceEditor(Form owner)
    {
        owner.Hide();
        AvaloniaRuntime.RunOnUiThread(() =>
        {
            var window = new EditorInterfaceWindow();
            window.Closed += (_, _) => owner.BeginInvoke(owner.Show);
            window.Show();
        });
    }
}
