using CryBits.Transport.Abstractions;
using LiteNetLib;

namespace CryBits.Client.Framework.Network.Transport;

public class UdpClientTransport : IClientTransport
{
    private readonly EventBasedNetListener _listener;
    private readonly NetManager _netManager;
    private NetPeer? _peer;

    /// <summary>Latest measured round-trip latency in milliseconds.</summary>
    public static int Latency;

    public bool IsConnected => _peer?.ConnectionState == ConnectionState.Connected;

    public event Action? OnConnected;
    public event Action? OnDisconnected;
    public event Action<byte[]>? OnDataReceived;

    public UdpClientTransport()
    {
        _listener = new EventBasedNetListener();
        _netManager = new NetManager(_listener);

        _listener.NetworkReceiveEvent += (_, reader, _, _) =>
        {
            var bytes = reader.GetRemainingBytes();
            OnDataReceived?.Invoke(bytes);
            reader.Recycle();
        };

        _listener.PeerDisconnectedEvent += (_, _) =>
        {
            _peer = null;
            OnDisconnected?.Invoke();
        };

        _listener.NetworkLatencyUpdateEvent += (_, latency) => Latency = latency;

        _listener.PeerConnectedEvent += _ => OnConnected?.Invoke();
    }

    public void Connect(string address, int port, string key)
    {
        _netManager.Start();
        _peer = _netManager.Connect(address, port, key);
        var waitTimer = Environment.TickCount64;
        while (!IsConnected && Environment.TickCount64 <= waitTimer + 1000)
            _netManager.PollEvents();
    }

    public void Disconnect()
    {
        _peer?.Disconnect();
        _netManager.Stop();
    }

    public void Poll() => _netManager.PollEvents();

    public void Send(byte[] data, DeliveryMethod delivery)
    {
        _peer?.Send(data, delivery);
    }
}
