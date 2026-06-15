using LiteNetLib;

namespace CryBits.Transport.Abstractions;

public interface IClientTransport
{
    void Connect(string address, int port, string key);
    void Disconnect();
    void Poll();
    void Send(byte[] data, DeliveryMethod delivery);
    bool IsConnected { get; }

    event Action? OnConnected;
    event Action? OnDisconnected;
    event Action<byte[]>? OnDataReceived;
}
