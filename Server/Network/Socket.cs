using CryBits.Server.Entities;
using LiteNetLib;
using static CryBits.Globals;

namespace CryBits.Server.Network;

internal static class Socket
{
    public static NetManager Device;
    private static EventBasedNetListener _listener;

    public static void Init()
    {
        _listener = new EventBasedNetListener();
        Device = new NetManager(_listener);

        _listener.ConnectionRequestEvent += request =>
        {
            if (Device.ConnectedPeersCount < Config.MaxPlayers)
                request.AcceptIfKey(Config.GameName);
            else
                request.Reject();
        };

        _listener.PeerConnectedEvent += peer =>
        {
            Account.List.Add(new Account(peer));
        };

        _listener.PeerDisconnectedEvent += (peer, _) =>
        {
            Account.List.Find(x => x.Connection == peer)?.Leave();
        };

        _listener.NetworkReceiveEvent += (peer, reader, _, _) =>
        {
            var account = Account.List.Find(x => x.Connection == peer);
            Receive.Handle(account, reader);
            reader.Recycle();
        };

        Device.Start(Config.Port);
    }

    public static void HandleData() => Device.PollEvents();
}