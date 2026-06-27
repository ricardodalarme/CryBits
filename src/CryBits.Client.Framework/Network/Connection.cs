using CryBits.Client.Framework.Network.Transport;
using CryBits.Protocol;
using CryBits.Protocol.Packets.Client;
using CryBits.Transport;
using CryBits.Transport.Abstractions;
using static CryBits.Definitions.Globals;

namespace CryBits.Client.Framework.Network;

/// <summary>
/// Shared network connection used by both the game client and the editor.
/// A thin facade over <see cref="IClientTransport"/> that handles event subscription
/// and delegates to <see cref="PacketDispatcher"/>.
/// </summary>
public class Connection
{
    public static Connection Instance { get; private set; } = null!;

    private readonly IClientTransport _transport;

    /// <summary>Latest measured round-trip latency in milliseconds.</summary>
    public static int Latency => UdpClientTransport.Latency;

    public Connection(IClientTransport transport)
    {
        _transport = transport;
        Instance = this;
    }

    public void Start(Action onDisconnected)
    {
        _transport.OnConnected += () => { };
        _transport.OnDisconnected += () => onDisconnected();
        _transport.OnDataReceived += bytes => PacketDispatcher.Dispatch(bytes);
    }

    public void Disconnect() => _transport.Disconnect();

    public void Poll() => _transport.Poll();

    public bool IsConnected => _transport.IsConnected;

    public void Send(byte[] data, DeliveryChannel delivery) => _transport.Send(data, delivery);

    public void SendPacket<T>(T packet, DeliveryChannel delivery = DeliveryChannel.ReliableOrdered) where T : IClientPacket
    {
        _transport.Send(PacketSerializer.Serialize<IClientPacket>(packet), delivery);
    }

    public bool TryConnect(string address, int port, string key)
    {
        if (IsConnected) return true;
        _transport.Connect(address, port, key);
        return IsConnected;
    }

    public bool TryConnect()
    {
        if (IsConnected) return true;
        _transport.Connect("localhost", Config.Port, Config.GameName);
        return IsConnected;
    }
}
