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
        // Define a latência
        Socket.Latency = Environment.TickCount - Socket.LatencySend;
    }

    internal static void Alert(NetDataReader data)
    {
        // Mostra a mensagem
        Utils.Alert.Show(data.GetString());
    }

    internal static void Connect()
    {
        // Reseta os valores
        PanelsEvents.SelectCharacter = 0;
        Class.List = new Dictionary<Guid, Class>();

        // Abre o painel de seleção de personagens
        PanelsEvents.MenuClose();
        Panels.SelectCharacter.Visible = true;
    }
}
