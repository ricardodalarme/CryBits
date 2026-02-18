using System;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;

namespace CryBits.Editors.AvaloniaUI;

internal static class AvaloniaRuntime
{
    private static readonly object SyncRoot = new();
    private static readonly ManualResetEventSlim AppReady = new(false);
    private static bool _started;

    public static void Initialize()
    {
        lock (SyncRoot)
        {
            if (_started) return;

            _started = true;

            var thread = new Thread(() =>
            {
                AppBuilder.Configure<App>()
                    .UsePlatformDetect()
                    .LogToTrace()
                    .StartWithClassicDesktopLifetime(Array.Empty<string>(), desktop =>
                    {
                        desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                    });
            });

            // STA is only meaningful on Windows (COM/WinForms interop)
            if (OperatingSystem.IsWindows())
                thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();
        }

        AppReady.Wait();
    }

    public static void RunOnUiThread(Action action)
    {
        Initialize();
        Dispatcher.UIThread.Post(action);
    }

    internal static void NotifyAppReady()
    {
        AppReady.Set();
    }
}
