using System;
using Avalonia;
using CryBits.Editors.AvaloniaUI.Forms;
using CryBits.Editors.Forms;

namespace CryBits.Editors.AvaloniaUI;

internal static class AvaloniaDataLauncher
{
    private static readonly object SyncRoot = new();
    private static bool _initialized;

    public static void Initialize()
    {
        lock (SyncRoot)
        {
            if (_initialized) return;

            BuildAvaloniaApp().SetupWithoutStarting();
            _initialized = true;
        }
    }

    public static void OpenDataEditor(EditorMaps owner)
    {
        Initialize();
        owner.Hide();

        var window = new EditorDataWindow();
        window.Closed += (_, _) =>
        {
            if (!owner.IsDisposed && owner.IsHandleCreated)
                owner.BeginInvoke(new Action(owner.Show));
        };
        window.Show();
    }

    private static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
    }
}
