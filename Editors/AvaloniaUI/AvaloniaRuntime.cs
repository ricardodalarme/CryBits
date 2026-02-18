using System;
using System.Threading;
using Avalonia;
using Avalonia.Threading;

namespace CryBits.Editors.AvaloniaUI;

internal static class AvaloniaRuntime
{
    private static readonly ManualResetEventSlim AppReady = new(false);

    /// <summary>
    /// Builds and returns the AppBuilder. Must be called on the main thread.
    /// The caller is responsible for invoking StartWithClassicDesktopLifetime.
    /// </summary>
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();

    /// <summary>
    /// Called from App.OnFrameworkInitializationCompleted to signal the loop thread.
    /// </summary>
    internal static void NotifyAppReady()
    {
        AppReady.Set();
    }

    /// <summary>
    /// Waits until Avalonia is fully initialised (blocks the calling background thread).
    /// </summary>
    public static void WaitUntilReady() => AppReady.Wait();

    public static void RunOnUiThread(Action action)
    {
        Dispatcher.UIThread.Post(action);
    }
}
