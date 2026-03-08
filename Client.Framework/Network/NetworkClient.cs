using System;
using LiteNetLib;
using LiteNetLib.Utils;
using static CryBits.Globals;

namespace CryBits.Client.Framework.Network;

/// <summary>
/// Shared network client used by both the game client and the editor.
/// </summary>
public class NetworkClient
{
    public static NetworkClient Instance { get; } = new();

    private readonly NetManager _device;
    private readonly EventBasedNetListener _listener;
    public NetPeer? ServerPeer { get; private set; }

    // Connection data
    private const string Ip = "localhost";

    /// <summary>Latest measured round-trip latency in milliseconds.</summary>
    public static int Latency;
    /// <summary>Timestamp (in milliseconds) at which the most recent latency packet was sent.</summary>
    public static int LatencySend;

    public NetworkClient()
    {
        _listener = new EventBasedNetListener();
        _device = new NetManager(_listener);
    }

    public void Init(Action<NetPacketReader> onPacketReceived, Action onDisconnected)
    {
        _listener.NetworkReceiveEvent += (_, reader, _, _) =>
        {
            onPacketReceived(reader);
            reader.Recycle();
        };

        _listener.PeerDisconnectedEvent += (_, _) =>
        {
            ServerPeer = null;
            onDisconnected();
        };

        _device.Start();
    }

    public void Disconnect()
    {
        ServerPeer?.Disconnect();
    }

    public void HandleData() => _device.PollEvents();

    public bool IsConnected() => ServerPeer?.ConnectionState == ConnectionState.Connected;

    public bool TryConnect()
    {
        if (IsConnected()) return true;

        ServerPeer = _device.Connect(Ip, Config.Port, Config.GameName);

        var waitTimer = Environment.TickCount;
        while (!IsConnected() && Environment.TickCount <= waitTimer + 1000)
            HandleData();

        return IsConnected();
    }
}
