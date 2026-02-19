using System;
using System.Collections.Generic;
using CryBits.Client.Framework.Constants;
using CryBits.Client.UI.Events;
using CryBits.Entities;
using LiteNetLib.Utils;

namespace CryBits.Client.Network.Handlers;

internal static class AuthHandler
{
    internal static void Latency()
    {
        // Update latency measurement
        Socket.Latency = Environment.TickCount - Socket.LatencySend;
    }

    internal static void Alert(NetDataReader data)
    {
        // Show alert message
        Utils.Alert.Show(data.GetString());
    }

    internal static void Connect()
    {
        // Reset client-side character selection state
        PanelsEvents.SelectCharacter = 0;
        Class.List = [];

        // Open character selection panel
        PanelsEvents.MenuClose();
        Panels.SelectCharacter.Visible = true;
    }
}
