using System;
using System.Linq;
using CryBits.Editors.AvaloniaUI;
using LiteNetLib;
using static CryBits.Globals;

namespace CryBits.Editors.Network;

internal static class Socket
{
    public static NetManager Device;
    private static EventBasedNetListener _listener;
    private static NetPeer _serverPeer;

    // Connection data
    private const string Ip = "localhost";

    public static NetPeer ServerPeer => _serverPeer;

    public static void Init()
    {
        _listener = new EventBasedNetListener();
        Device = new NetManager(_listener);

        _listener.NetworkReceiveEvent += (_, reader, _, _) =>
        {
            Receive.Handle(reader);
            reader.Recycle();
        };

        _listener.PeerDisconnectedEvent += (_, _) =>
        {
            _serverPeer = null;
            Leave();
        };

        Device.Start();
    }

    public static void Disconnect()
    {
        _serverPeer?.Disconnect();
    }

    public static void HandleData() => Device.PollEvents();

    public static bool IsConnected() => _serverPeer?.ConnectionState == ConnectionState.Connected;

    public static bool TryConnect()
    {
        if (IsConnected()) return true;

        _serverPeer = Device.Connect(Ip, Config.Port, Config.GameName);

        var waitTimer = Environment.TickCount;
        while (!IsConnected() && Environment.TickCount <= waitTimer + 1000)
            HandleData();

        return IsConnected();
    }

    private static void Leave()
    {
        // Fecha todas as janelas abertas e abre o menu de login
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            if (Avalonia.Application.Current?.ApplicationLifetime is
                Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
            {
                foreach (var win in desktop.Windows.ToArray())
                    win.Close();
            }
            AvaloniaLoginLauncher.ShowLogin();
        });
    }
}