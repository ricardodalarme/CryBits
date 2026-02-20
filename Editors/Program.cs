using System;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Persistence.Repositories;
using CryBits.Editors.AvaloniaUI;
using CryBits.Editors.Logic;
using CryBits.Editors.Network;
using EditorToolsRepository = CryBits.Editors.Library.Repositories.ToolsRepository;

namespace CryBits.Editors;

internal static class Program
{
    /// <summary>Indicates whether the application main loop is running.</summary>
    public static bool Working = true;

    // Measured frames per second.
    public static short Fps;

    private static void Main()
    {
        // Ensure required directories exist.
        Directories.Create();

        // Load preferences.
        OptionsRepository.Read();
        EditorToolsRepository.Read();

        // Initialize subsystems
        Socket.Init();
        Sound.Load();

        // Start the game loop on a background thread.
        // It will block on AvaloniaRuntime.WaitUntilReady() until Avalonia is up.
        var loopThread = new Thread(() =>
        {
            // Wait until Avalonia is fully initialised on the main thread
            AvaloniaRuntime.WaitUntilReady();

            // Show login window and start the editor loop
            AvaloniaLoginLauncher.ShowLogin();
            Loop.Init();
        });
        loopThread.IsBackground = true;
        loopThread.Start();

        // Run Avalonia on the main thread (required by macOS/Cocoa and Linux/X11)
        AvaloniaRuntime.BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(Array.Empty<string>(),
                desktop => { desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown; });
    }

    public static void Close()
    {
        var waitTimer = Environment.TickCount;

        // Disconnect from network
        Socket.Disconnect();

        // Wait until the player is disconnected
        while (Socket.IsConnected() && Environment.TickCount <= waitTimer + 1000)
            Thread.Sleep(10);

        // Close the application
        Working = false;
    }
}
