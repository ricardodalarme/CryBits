using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Network;
using CryBits.Client.Framework.Network.Transport;
using CryBits.Client.Framework.Persistence.Repositories;
using CryBits.Definitions.Catalog;
using CryBits.Editors.Core;
using CryBits.Editors.Forms.Login;
using CryBits.Editors.Graphics;
using CryBits.Editors.Network;
using CryBits.Editors.Network.Handlers;

using static CryBits.Definitions.Globals;

namespace CryBits.Editors;

internal static class Program
{
    /// <summary>
    /// The active MonoGame <see cref="Microsoft.Xna.Framework.Graphics.GraphicsDevice"/> in the
    /// editor process. Set by the first editor window that creates an <see cref="EditorGame"/>;
    /// used by <see cref="Forms.UI.EditorUILayoutWindow"/> and
    /// <see cref="Forms.UI.EditorUIThemeWindow"/> for their Iguina previews.
    /// </summary>
    public static Microsoft.Xna.Framework.Graphics.GraphicsDevice? SharedDevice { get; set; }

    public static void Main()
    {
        Directories.Create();
        OptionsRepository.Read();
        EditorGraphics.Initialize();

        // ── Create all infrastructure ──
        var audio = new AudioManager();
        var catalog = new DefinitionCatalog();
        var renderer = EditorGraphics.Renderer;
        var clientTransport = new UdpClientTransport();
        clientTransport.Connect("localhost", Config.Port, Config.GameName);
        var connection = new Connection(clientTransport);
        var sender = new PackageSender(connection);
        var shell = new EditorShell(catalog, audio, connection, sender, renderer);

        audio.LoadSounds();
        PacketDispatcher.Register(new AuthHandler(shell));
        PacketDispatcher.Register(new ContentHandler(catalog));

        // ── Start game loop ──
        connection.Start(onDisconnected: () => Leave(shell));
        var loopThread = new Thread(() =>
        {
            App.WaitUntilReady();

            LoginWindow.Open(shell);
            shell.Loop.Run().GetAwaiter().GetResult();
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

    private static void Leave(EditorShell shell)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            if (Application.Current?.ApplicationLifetime is
                IClassicDesktopStyleApplicationLifetime desktop)
                foreach (var win in desktop.Windows.ToArray())
                    win.Close();

            LoginWindow.Open(shell);
        });
    }
}
