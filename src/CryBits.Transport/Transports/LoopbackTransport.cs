using CryBits.Transport.Abstractions;
using System.Threading.Channels;

namespace CryBits.Transport.Transports;

public sealed class LoopbackServerTransport : ITransport
{
    private readonly ChannelReader<byte[]> _clientToServer;
    private readonly ChannelWriter<byte[]> _serverToClient;
    private static readonly Guid LocalSessionId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public bool IsRunning { get; private set; }

    public event Action<Guid>? OnConnected;
    public event Action<Guid>? OnDisconnected;
    public event Action<Guid, byte[]>? OnDataReceived;

    internal LoopbackServerTransport(ChannelReader<byte[]> clientToServer, ChannelWriter<byte[]> serverToClient)
    {
        _clientToServer = clientToServer;
        _serverToClient = serverToClient;
    }

    public void Start(int port, string gameName, byte maxPlayers)
    {
        if (IsRunning) return;
        IsRunning = true;
        OnConnected?.Invoke(LocalSessionId);
    }

    public void Stop()
    {
        if (!IsRunning) return;
        IsRunning = false;
        OnDisconnected?.Invoke(LocalSessionId);
    }

    public void Poll()
    {
        while (IsRunning && _clientToServer.TryRead(out var bytes))
            OnDataReceived?.Invoke(LocalSessionId, bytes);
    }

    public void Send(Guid sessionId, byte[] data, DeliveryChannel delivery)
    {
        _serverToClient.TryWrite(data);
    }

    public void Disconnect(Guid sessionId)
    {
        Stop();
    }
}

public sealed class LoopbackClientTransport : IClientTransport
{
    private readonly ChannelReader<byte[]> _serverToClient;
    private readonly ChannelWriter<byte[]> _clientToServer;

    public bool IsConnected { get; private set; }

    public event Action? OnConnected;
    public event Action? OnDisconnected;
    public event Action<byte[]>? OnDataReceived;

    internal LoopbackClientTransport(ChannelReader<byte[]> serverToClient, ChannelWriter<byte[]> clientToServer)
    {
        _serverToClient = serverToClient;
        _clientToServer = clientToServer;
    }

    public void Connect(string address, int port, string key)
    {
        if (IsConnected) return;
        IsConnected = true;
        OnConnected?.Invoke();
    }

    public void Disconnect()
    {
        if (!IsConnected) return;
        IsConnected = false;
        OnDisconnected?.Invoke();
    }

    public void Poll()
    {
        while (_serverToClient.TryRead(out var bytes))
            OnDataReceived?.Invoke(bytes);
    }

    public void Send(byte[] data, DeliveryChannel delivery)
    {
        if (IsConnected)
            _clientToServer.TryWrite(data);
    }
}
