using LiteNetLib;
using static CryBits.Globals;

namespace CryBits.Client.Framework.Network;

/// <summary>
/// Shared network client used by both the game client and the editor.
/// </summary>
public class NetworkClient
{
    public static NetworkClient Instance { get; } = new();

    private readonly NetManager _netManager;
    private readonly EventBasedNetListener _listener;
    public NetPeer? ServerPeer { get; private set; }

    // Connection data
    private const string Ip = "localhost";

    /// <summary>Latest measured round-trip latency in milliseconds.</summary>
    public static int Latency;

    public NetworkClient()
    {
        _listener = new EventBasedNetListener();
        _netManager = new NetManager(_listener);
    }

    public void Start(Action onDisconnected)
    {
        _listener.NetworkReceiveEvent += (_, reader, _, _) =>
        {
            PacketDispatcher.Dispatch(reader);
            reader.Recycle();
        };

        _listener.PeerDisconnectedEvent += (_, _) =>
        {
            ServerPeer = null;
            onDisconnected();
        };

        _listener.NetworkLatencyUpdateEvent += (_, latency) => Latency = latency;

        _netManager.Start();
    }

    public void Disconnect()
    {
        ServerPeer?.Disconnect();
    }

    public void HandleData() => _netManager.PollEvents();

    public bool IsConnected() => ServerPeer?.ConnectionState == ConnectionState.Connected;

    public bool TryConnect()
    {
        if (IsConnected()) return true;

        ServerPeer = _netManager.Connect(Ip, Config.Port, Config.GameName);

        var waitTimer = Environment.TickCount;
        while (!IsConnected() && Environment.TickCount <= waitTimer + 1000)
            HandleData();

        return IsConnected();
    }
}
