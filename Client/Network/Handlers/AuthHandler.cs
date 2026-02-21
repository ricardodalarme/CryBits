using System;
using CryBits.Client.Framework.Constants;
using CryBits.Client.UI.Events;
using CryBits.Entities;
using CryBits.Packets.Server;

namespace CryBits.Client.Network.Handlers;

internal static class AuthHandler
{
    [PacketHandler]
    internal static void Latency(LatencyPacket _)
    {
        // Update latency measurement
        Socket.Latency = Environment.TickCount - Socket.LatencySend;
    }

    [PacketHandler]
    internal static void Alert(AlertPacket packet)
    {
        // Show alert message
        Utils.Alert.Show(packet.Message);
    }

    [PacketHandler]
    internal static void Connect(ConnectPacket _)
    {
        // Reset client-side character selection state
        PanelsEvents.SelectCharacter = 0;
        Class.List = [];

        // Open character selection panel
        PanelsEvents.MenuClose();
        Panels.SelectCharacter.Visible = true;
    }
}
