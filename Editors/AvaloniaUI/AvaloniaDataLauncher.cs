using System;
using CryBits.Editors.AvaloniaUI.Forms;
using CryBits.Editors.Forms;

namespace CryBits.Editors.AvaloniaUI;

internal static class AvaloniaDataLauncher
{
    public static void OpenDataEditor(EditorMaps owner)
    {
        owner.Hide();
        AvaloniaRuntime.RunOnUiThread(() =>
        {
            var window = new EditorDataWindow();
            window.Closed += (_, _) =>
            {
                if (!owner.IsDisposed && owner.IsHandleCreated)
                    owner.BeginInvoke(new Action(owner.Show));
            };
            window.Show();
        });
    }

}
