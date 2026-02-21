using System;
using CryBits.Client.Framework.Constants;
using CryBits.Enums;
using CryBits.Packets.Client;

namespace CryBits.Client.Network.Senders;

internal static class AuthSender
{
    public static void Latency()
    {
        Send.Packet(ClientPacket.Latency, new LatencyPacket());

        // Record latency send timestamp
        Socket.LatencySend = Environment.TickCount;
    }

    public static void Connect() => Send.Packet(ClientPacket.Connect, new ConnectPacket
    {
        Username = TextBoxes.ConnectUsername.Text,
        Password = TextBoxes.ConnectPassword.Text,
        IsClientAccess = false
    });

    public static void Register() => Send.Packet(ClientPacket.Register, new RegisterPacket
    {
        Username = TextBoxes.RegisterUsername.Text,
        Password = TextBoxes.RegisterPassword.Text
    });
}
