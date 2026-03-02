using System;
using CryBits.Client.UI.Menu;
using CryBits.Client.UI.Menu.Views;
using CryBits.Entities;
using CryBits.Packets.Server;

namespace CryBits.Client.Network.Handlers;

internal class AuthHandler
{
    [PacketHandler]
    internal void Latency(LatencyPacket _)
    {
        // Update latency measurement
        NetworkClient.Latency = Environment.TickCount - NetworkClient.LatencySend;
    }

    [PacketHandler]
    internal void Alert(AlertPacket packet)
    {
        // Show alert message
        Utils.Alert.Show(packet.Message);
    }

    [PacketHandler]
    internal void Connect(ConnectPacket _)
    {
        // Reset client-side character selection state
        SelectCharacterView.CurrentCharacter = 0;
        Class.List = [];

        // Open character selection panel
        MenuScreen.CloseMenus();
        SelectCharacterView.SelectCharacterPanel.Visible = true;
    }
}
