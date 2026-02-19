using System;
using CryBits.Client.Entities;
using CryBits.Client.UI;
using CryBits.Client.Utils;
using LiteNetLib;
using static CryBits.Globals;

namespace CryBits.Client.Network;

internal static class Socket
{
    public static NetManager Device;
    private static EventBasedNetListener _listener;
    private static NetPeer _serverPeer;

    // Connection data
    private const string Ip = "localhost";

    /// <summary>Latest measured round-trip latency in milliseconds.</summary>
    public static int Latency;
    public static int LatencySend;

    public static NetPeer ServerPeer => _serverPeer;

    public static void Init()
    {
        _listener = new EventBasedNetListener();
        Device = new NetManager(_listener);

        _listener.NetworkReceiveEvent += (_, reader, _, _) =>
        {
            Receive.Handle(reader);
            reader.Recycle();
        };

        _listener.PeerDisconnectedEvent += (_, _) =>
        {
            _serverPeer = null;
            if (Player.Me != null) Player.Me.Leave();
            Window.OpenMenu();
        };

        Device.Start();
    }

    public static void Disconnect()
    {
        _serverPeer?.Disconnect();
    }

    public static void HandleData() => Device.PollEvents();

    public static bool IsConnected() => _serverPeer?.ConnectionState == ConnectionState.Connected;

    public static bool TryConnect()
    {
        if (IsConnected()) return true;

        _serverPeer = Device.Connect(Ip, Config.Port, Config.GameName);

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
