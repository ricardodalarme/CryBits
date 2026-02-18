using System.Windows.Forms;
using CryBits.Editors.AvaloniaUI.Forms;
using Map = CryBits.Entities.Map.Map;

namespace CryBits.Editors.AvaloniaUI;

internal static class AvaloniaClassesLauncher
{
    public static void OpenClassesEditor(Form owner)
    {
        if (Map.List.Count == 0)
        {
            MessageBox.Show("It must have at least one map registered before editing classes.");
            return;
        }

        owner.Hide();
        AvaloniaRuntime.RunOnUiThread(() =>
        {
            var window = new EditorClassesWindow();
            window.Closed += (_, _) => owner.BeginInvoke(owner.Show);
            window.Show();
        });
    }
}
