using CryBits.Server.Entities;
using CryBits.Server.World;
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
            GameWorld.Current.Accounts.Add(new Account(peer));
        };

        _listener.PeerDisconnectedEvent += (peer, _) =>
        {
            GameWorld.Current.Accounts.Find(x => x.Connection == peer)?.Leave();
        };

        _listener.NetworkReceiveEvent += (peer, reader, _, _) =>
        {
            var account = GameWorld.Current.Accounts.Find(x => x.Connection == peer);
            PacketDispatcher.Dispatch(account, reader);
            reader.Recycle();
        };

        Device.Start(Config.Port);
    }

    public static void HandleData() => Device.PollEvents();
}
