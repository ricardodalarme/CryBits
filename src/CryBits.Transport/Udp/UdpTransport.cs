using CryBits.Transport.Abstractions;
using LiteNetLib;
using static CryBits.Definitions.Globals;

namespace CryBits.Transport.Udp;

public sealed class UdpTransport : ITransport
{
    private EventBasedNetListener? _listener;
    private NetManager? _device;
    private readonly Dictionary<Guid, NetPeer> _peers = [];

    public bool IsRunning => _device is { IsRunning: true };

    public event Action<Guid>? OnConnected;
    public event Action<Guid>? OnDisconnected;
    public event Action<Guid, byte[]>? OnDataReceived;

    public void Start(int port)
    {
        _listener = new EventBasedNetListener();
        _device = new NetManager(_listener);

        _listener.ConnectionRequestEvent += request =>
        {
            if (_device.ConnectedPeersCount < Config.MaxPlayers)
                request.AcceptIfKey(Config.GameName);
            else
                request.Reject();
        };

        _listener.PeerConnectedEvent += peer =>
        {
            var sessionId = Guid.NewGuid();
            _peers[sessionId] = peer;
            OnConnected?.Invoke(sessionId);
        };

        _listener.PeerDisconnectedEvent += (peer, _) =>
        {
            var pair = _peers.FirstOrDefault(x => x.Value == peer);
            if (pair.Value == null) return;
            _peers.Remove(pair.Key);
            OnDisconnected?.Invoke(pair.Key);
        };

        _listener.NetworkReceiveEvent += (peer, reader, _, _) =>
        {
            var pair = _peers.FirstOrDefault(x => x.Value == peer);
            if (pair.Value == null) return;
            var bytes = reader.GetRemainingBytes();
            OnDataReceived?.Invoke(pair.Key, bytes);
            reader.Recycle();
        };

        _device.Start(port);
    }

    public void Stop()
    {
        _device?.Stop();
        _peers.Clear();
    }

    public void Poll() => _device?.PollEvents();

    public void Send(Guid sessionId, byte[] data, DeliveryMethod delivery)
    {
        if (_peers.TryGetValue(sessionId, out var peer))
            peer.Send(data, delivery);
    }

    public void Disconnect(Guid sessionId)
    {
        if (_peers.TryGetValue(sessionId, out var peer))
        {
            peer.Disconnect();
            _peers.Remove(sessionId);
        }
    }
}
