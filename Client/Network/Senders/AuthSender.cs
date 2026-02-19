using System;
using CryBits.Client.Framework.Constants;
using CryBits.Enums;
using LiteNetLib.Utils;

namespace CryBits.Client.Network.Senders;

internal static class AuthSender
{
    public static void Latency()
    {
        var data = new NetDataWriter();

        data.Put((byte)ClientPacket.Latency);
        Send.Packet(data);

        // Record latency send timestamp
        Socket.LatencySend = Environment.TickCount;
    }

    public static void Connect()
    {
        var data = new NetDataWriter();

        data.Put((byte)ClientPacket.Connect);
        data.Put(TextBoxes.ConnectUsername.Text);
        data.Put(TextBoxes.ConnectPassword.Text);
        data.Put(false); // Client access flag
        Send.Packet(data);
    }

    public static void Register()
    {
        var data = new NetDataWriter();

        data.Put((byte)ClientPacket.Register);
        data.Put(TextBoxes.RegisterUsername.Text);
        data.Put(TextBoxes.RegisterPassword.Text);
        Send.Packet(data);
    }
}
