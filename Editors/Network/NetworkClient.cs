using System;
using System.Linq;
using CryBits.Client.Framework.Network;
using CryBits.Editors.Forms;
using LiteNetLib;
using static CryBits.Globals;

namespace CryBits.Editors.Network;

internal class NetworkClient
{
    public static NetworkClient Instance { get; } = new();

    private NetManager _device;
    private EventBasedNetListener _listener;
    private NetPeer _serverPeer;

    // Connection data
    private const string Ip = "localhost";

    public NetPeer ServerPeer => _serverPeer;

    public void Init()
    {
        _listener = new EventBasedNetListener();
        _device = new NetManager(_listener);

        _listener.NetworkReceiveEvent += (_, reader, _, _) =>
        {
            PacketDispatcher.Dispatch(reader);
            reader.Recycle();
        };

        _listener.PeerDisconnectedEvent += (_, _) =>
        {
            _serverPeer = null;
            Leave();
        };

        _device.Start();
    }

    public void Disconnect()
    {
        _serverPeer?.Disconnect();
    }

    public void HandleData() => _device.PollEvents();

    public bool IsConnected() => _serverPeer?.ConnectionState == ConnectionState.Connected;

    public bool TryConnect()
    {
        if (IsConnected()) return true;

        _serverPeer = _device.Connect(Ip, Config.Port, Config.GameName);

        var waitTimer = Environment.TickCount;
        while (!IsConnected() && Environment.TickCount <= waitTimer + 1000)
            HandleData();

        return IsConnected();
    }

    private static void Leave()
    {
        // Close all open windows and show the login menu.
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            if (Avalonia.Application.Current?.ApplicationLifetime is
                Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
            {
                foreach (var win in desktop.Windows.ToArray())
                    win.Close();
            }

            LoginWindow.Open();
        });
    }
}
