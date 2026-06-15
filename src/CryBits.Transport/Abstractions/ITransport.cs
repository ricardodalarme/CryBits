using LiteNetLib;

namespace CryBits.Transport.Abstractions;

public interface ITransport
{
    void Start(int port);
    void Stop();
    void Poll();
    void Send(Guid sessionId, byte[] data, DeliveryMethod delivery);
    void Disconnect(Guid sessionId);
    bool IsRunning { get; }

    event Action<Guid>? OnConnected;
    event Action<Guid>? OnDisconnected;
    event Action<Guid, byte[]>? OnDataReceived;
}
