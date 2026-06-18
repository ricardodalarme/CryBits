namespace CryBits.Transport.Abstractions;

public interface IClientTransport
{
    void Connect(string address, int port, string key);
    void Disconnect();
    void Poll();
    void Send(byte[] data, DeliveryChannel delivery);
    bool IsConnected { get; }

    event Action? OnConnected;
    event Action? OnDisconnected;
    event Action<byte[]>? OnDataReceived;
}
