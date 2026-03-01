using System;
using CryBits.Client.Framework.Constants;
using CryBits.Packets.Client;

namespace CryBits.Client.Network.Senders;

internal class AuthSender(PacketSender packetSender)
{
    public static AuthSender Instance { get; } = new(PacketSender.Instance);

    public void Latency()
    {
        packetSender.Packet(new LatencyPacket());

        // Record latency send timestamp
        NetworkClient.LatencySend = Environment.TickCount;
    }

    public void Connect() => packetSender.Packet(new ConnectPacket
    {
        Username = TextBoxes.ConnectUsername.Text,
        Password = TextBoxes.ConnectPassword.Text,
        IsClientAccess = false
    });

    public void Register() => packetSender.Packet(new RegisterPacket
    {
        Username = TextBoxes.RegisterUsername.Text,
        Password = TextBoxes.RegisterPassword.Text
    });
}
