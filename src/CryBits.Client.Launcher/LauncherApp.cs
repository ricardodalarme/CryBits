using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Network;
using CryBits.Client.Framework.Network.Transport;
using CryBits.Client.Framework.Persistence.Repositories;
using CryBits.Client.Launcher.Offline;
using CryBits.Client.Rendering;
using CryBits.Definitions.Catalog;
using CryBits.Protocol.Serialization;
using CryBits.Simulation;
using CryBits.Transport.Transports;
using System.Diagnostics;

namespace CryBits.Client.Launcher;

using static CryBits.Definitions.Globals;

public sealed class LauncherApp : IDisposable
{
    private readonly SpriteBatch _window;
    private readonly Connection _connection;
    private readonly EmbeddedHostRunner? _hostRunner;
    private readonly Game _game;
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

    public LauncherApp(bool offline)
    {
        // Bootstrap
        Directories.Create();
        OptionsRepository.Read();
        RegisterComponentTypes();

        // Window
        _window = new SpriteBatch();
        _window.WindowCloseRequested += () => _window.RenderWindow.Close();
        _window.WindowFocusChanged += _ => { };

        // Network
        if (offline)
        {
            _hostRunner = new EmbeddedHostRunner();
            _hostRunner.Start();
            _connection = _hostRunner.ClientConnection;
        }
        else
        {
            var transport = new UdpClientTransport();
            transport.Connect("localhost", Config.Port, Config.GameName);
            _connection = new Connection(transport);
        }
        _game = new Game(_window, _connection);
        _connection.Start(onDisconnected: () => _game.EndSession());
    }

    public void Run()
    {
        short fps = 0;
        long timer1000 = 0;

        while (_window.RenderWindow.IsOpen)
        {
            try
            {
                _connection.Poll();
                _window.RenderWindow.DispatchEvents();

                var deltaTime = (float)_stopwatch.Elapsed.TotalSeconds;
                _stopwatch.Restart();

                _game.Update(deltaTime);
                _game.Render(_window.RenderWindow);

                if (timer1000 < Environment.TickCount64)
                {
                    Game.Fps = fps;
                    fps = 0;
                    timer1000 = Environment.TickCount64 + 1000;
                }
                else
                {
                    fps++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Main loop threw an exception: {ex}");
            }
        }
    }

    public void Dispose()
    {
        _game.Dispose();
        _hostRunner?.Stop();
        _hostRunner?.Dispose();
        _connection.Disconnect();
    }

    private static void RegisterComponentTypes()
    {
        ComponentTypes.RegisterDefault();
        ComponentTypeRegistry.Register<Components.NetworkId>(18);
    }
}
