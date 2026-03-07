using CryBits.Packets.Client;

namespace CryBits.Client.Network.Senders;

internal class AuthSender(PacketSender packetSender)
{
    public static AuthSender Instance { get; } = new(PacketSender.Instance);

    public void Connect(string username, string password) => packetSender.Packet(new ConnectPacket
    {
        Username = username,
        Password = password,
        IsClientAccess = false
    });

    public void Register(string username, string password) => packetSender.Packet(new RegisterPacket
    {
        Username = username,
        Password = password
    });
}
