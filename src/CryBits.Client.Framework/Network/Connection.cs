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
public class Connection(IClientTransport transport)
{
    public static Connection Instance { get; set; } = null!;

    /// <summary>Latest measured round-trip latency in milliseconds.</summary>
    public static int Latency => UdpClientTransport.Latency;

    public void Start(Action onDisconnected)
    {
        transport.OnConnected += () => { };
        transport.OnDisconnected += () => onDisconnected();
        transport.OnDataReceived += bytes => PacketDispatcher.Dispatch(bytes);
    }

    public void Disconnect() => transport.Disconnect();

    public void Poll() => transport.Poll();

    public bool IsConnected => transport.IsConnected;
    
    public void SendPacket<T>(T packet, DeliveryChannel delivery = DeliveryChannel.ReliableOrdered) where T : IClientPacket
    {
        transport.Send(PacketSerializer.Serialize<IClientPacket>(packet), delivery);
    }

    public bool TryConnect()
    {
        if (IsConnected) return true;
        transport.Connect("localhost", Config.Port, Config.GameName);
        return IsConnected;
    }
}
