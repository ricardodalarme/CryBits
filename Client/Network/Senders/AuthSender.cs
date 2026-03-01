using System;
using CryBits.Client.Framework.Constants;
using CryBits.Packets.Client;

namespace CryBits.Client.Network.Senders;

internal static class AuthSender
{
    public static void Latency()
    {
        PacketSender.Packet(new LatencyPacket());

        // Record latency send timestamp
        NetworkClient.LatencySend = Environment.TickCount;
    }

    public static void Connect() => PacketSender.Packet(new ConnectPacket
    {
        Username = TextBoxes.ConnectUsername.Text,
        Password = TextBoxes.ConnectPassword.Text,
        IsClientAccess = false
    });

    public static void Register() => PacketSender.Packet(new RegisterPacket
    {
        Username = TextBoxes.RegisterUsername.Text,
        Password = TextBoxes.RegisterPassword.Text
    });
}
