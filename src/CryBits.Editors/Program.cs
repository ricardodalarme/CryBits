using Avalonia;
using Avalonia.Controls;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Network;
using CryBits.Client.Framework.Network.Transport;
using CryBits.Client.Framework.Persistence.Repositories;
using CryBits.Definitions.Catalog;
using CryBits.Editors.Entities;
using CryBits.Editors.Forms.Login;
using CryBits.Editors.Logic;
using CryBits.Editors.Network;
using CryBits.Editors.Network.Handlers;
using static CryBits.Definitions.Globals;

namespace CryBits.Editors;

internal static class Program
{
    public static bool Working = true;
    public static short Fps;

    internal static DefinitionCatalog Catalog = null!;
    internal static PackageSender Sender = null!;
    internal static Loop EditorLoop = null!;

    private static void Main()
    {
        Directories.Create();
        OptionsRepository.Read();

        // ── Create all infrastructure ──
        var audio = new AudioManager();
        AudioManager.Instance = audio;
        var catalog = new DefinitionCatalog();
        Catalog = catalog;
        var clientTransport = new UdpClientTransport();
        clientTransport.Connect("localhost", Config.Port, Config.GameName);
        var connection = new Connection(clientTransport);
        Connection.Instance = connection;
        connection.Start(onDisconnected: Leave);
        var sender = new PackageSender(connection, catalog);
        PackageSender.Instance = Sender = sender;

        audio.LoadSounds();
        PacketDispatcher.Register(new AuthHandler());
        PacketDispatcher.Register(new ContentHandler(catalog));

        // ── Start game loop ──
        var loopThread = new Thread(() =>
        {
            App.WaitUntilReady();

            var loop = new Loop(MapInstance.Instance);
            EditorLoop = loop;
            Loop.Instance = loop;
            LoginWindow.Open();
            loop.Init();
        })
        { IsBackground = true };
        loopThread.Start();

        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime([],
                desktop => desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown);
    }

    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();

    private static void Leave()
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            if (Application.Current?.ApplicationLifetime is
                Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
                foreach (var win in desktop.Windows.ToArray())
                    win.Close();

            LoginWindow.Open();
        });
    }

    public static void Close()
    {
        var waitTimer = Environment.TickCount64;

        Connection.Instance.Disconnect();

        while (Connection.Instance.IsConnected && Environment.TickCount64 <= waitTimer + 1000)
            Thread.Sleep(10);

        Working = false;
    }
}
