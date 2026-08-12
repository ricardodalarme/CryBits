using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Network;
using CryBits.Client.Framework.Network.Transport;
using CryBits.Client.Framework.Persistence.Repositories;
using CryBits.Client.Launcher.Offline;
using CryBits.Protocol.Serialization;
using CryBits.Simulation;

using static CryBits.Definitions.Globals;

namespace CryBits.Client.Launcher;


public sealed class LauncherApp : IDisposable
{
    private readonly Connection _connection;
    private readonly EmbeddedHostRunner? _hostRunner;

    public LauncherApp(string[] args)
    {
        // Pre-flight launcher bootstrapping
        Directories.Create();
        OptionsRepository.Read();
        RegisterComponentTypes();

        // Setup host runner or network transport
        if (args.Contains("--offline"))
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
    }

    public void Run()
    {
        using var game = new Game(_connection);
        game.Run();
    }

    public void Dispose()
    {
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
