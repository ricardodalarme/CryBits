using System;
using System.Windows.Forms;
using CryBits.Editors.AvaloniaUI.Forms;
using CryBits.Editors.Forms;
using CryBits.Entities;

namespace CryBits.Editors.AvaloniaUI;

internal static class AvaloniaShopsLauncher
{
    public static void OpenShopsEditor(EditorMaps owner)
    {
        if (Item.List.Count == 0)
        {
            MessageBox.Show(@"It must have at least one item registered to open the store editor.");
            return;
        }

        owner.Hide();
        AvaloniaRuntime.RunOnUiThread(() =>
        {
            var window = new EditorShopsWindow();
            window.Closed += (_, _) =>
            {
                if (!owner.IsDisposed && owner.IsHandleCreated)
                    owner.BeginInvoke(new Action(owner.Show));
            };
            window.Show();
        });
    }
}
