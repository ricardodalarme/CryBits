using CryBits.Server.Systems;
using CryBits.Server.World;
using LiteNetLib;
using static CryBits.Globals;

namespace CryBits.Server.Network;

internal sealed class NetworkServer(CharacterSystem characterSystem)
{
    public static NetworkServer Instance { get; } = new(CharacterSystem.Instance);

    public NetManager Device { get; private set; }

    public void Init()
    {
        var listener = new EventBasedNetListener();
        Device = new NetManager(listener);

        listener.ConnectionRequestEvent += request =>
        {
            if (Device.ConnectedPeersCount < Config.MaxPlayers)
                request.AcceptIfKey(Config.GameName);
            else
                request.Reject();
        };

        listener.PeerConnectedEvent += peer => GameWorld.Current.Sessions.Add(new GameSession(peer));

        listener.PeerDisconnectedEvent += (peer, _) =>
        {
            var session = GameWorld.Current.Sessions.Find(x => x.Connection == peer);
            if (session == null) return;
            if (session.Character != null) characterSystem.Leave(session.Character);
            GameWorld.Current.Sessions.Remove(session);
        };

        listener.NetworkReceiveEvent += (peer, reader, _, _) =>
        {
            var session = GameWorld.Current.Sessions.Find(x => x.Connection == peer);
            PacketDispatcher.Dispatch(session, reader);
            reader.Recycle();
        };

        Device.Start(Config.Port);
    }

    public void HandleData() => Device.PollEvents();
}
