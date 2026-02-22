using CryBits.Server.Systems;
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

        _listener.PeerConnectedEvent += peer => { GameWorld.Current.Sessions.Add(new GameSession(peer)); };

        _listener.PeerDisconnectedEvent += (peer, _) =>
        {
            var session = GameWorld.Current.Sessions.Find(x => x.Connection == peer);
            if (session == null) return;
            if (session.Character != null) CharacterSystem.Leave(session.Character);
            GameWorld.Current.Sessions.Remove(session);
        };

        _listener.NetworkReceiveEvent += (peer, reader, _, _) =>
        {
            var session = GameWorld.Current.Sessions.Find(x => x.Connection == peer);
            PacketDispatcher.Dispatch(session, reader);
            reader.Recycle();
        };

        Device.Start(Config.Port);
    }

    public static void HandleData() => Device.PollEvents();
}
