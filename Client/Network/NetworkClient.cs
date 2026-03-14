using System;
using CryBits.Client.Framework.Network;
using CryBits.Client.UI;
using CryBits.Client.Utils;
using CryBits.Client.Worlds;
using LiteNetLib;
using static CryBits.Globals;

namespace CryBits.Client.Network;

internal class NetworkClient
{
    public static NetworkClient Instance { get; } = new();

    private readonly NetManager _device;
    private readonly EventBasedNetListener _listener;
    public NetPeer? ServerPeer { get; private set; }

    // Connection data
    private const string Ip = "localhost";

    /// <summary>Latest measured round-trip latency in milliseconds.</summary>
    public static int Latency;

    public NetworkClient()
    {
        _listener = new EventBasedNetListener();
        _device = new NetManager(_listener);
    }

    public void Init()
    {
        _listener.NetworkReceiveEvent += (_, reader, _, _) =>
        {
            PacketDispatcher.Dispatch(reader);
            reader.Recycle();
        };

        _listener.PeerDisconnectedEvent += (_, _) =>
        {
            ServerPeer = null;
            GameContext.Instance.Reset();
            Window.OpenMenu();
        };

        _listener.NetworkLatencyUpdateEvent += (_, latency) => Latency = latency;

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

        if (!IsConnected())
        {
            Alert.Show("The server is currently unavailable.");
            return false;
        }

        return true;
    }
}
