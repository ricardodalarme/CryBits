namespace CryBits.Transport.Abstractions;

public interface ITransport
{
    void Start(int port, string gameName, byte maxPlayers);
    void Stop();
    void Poll();
    void Send(Guid sessionId, byte[] data, DeliveryChannel delivery);
    void Disconnect(Guid sessionId);
    bool IsRunning { get; }

    event Action<Guid>? OnConnected;
    event Action<Guid>? OnDisconnected;
    event Action<Guid, byte[]>? OnDataReceived;
}
