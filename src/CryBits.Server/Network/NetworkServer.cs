using CryBits.Server.Core;
using CryBits.Server.Network.Handlers;
using LiteNetLib;
using static CryBits.Definitions.Globals;

namespace CryBits.Server.Network;

internal sealed class NetworkServer(AccountHandler accountHandler)
{
    public static NetworkServer Instance { get; } = new(AccountHandler.Instance);

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

        listener.PeerConnectedEvent += peer => WorldHost.Current.Sessions.Add(new GameSession(peer));

        listener.PeerDisconnectedEvent += (peer, _) =>
        {
            var session = WorldHost.Current.Sessions.Find(x => x.Connection == peer);
            if (session == null) return;
            if (session.Character is { } characterId) accountHandler.Leave(characterId);
            WorldHost.Current.Sessions.Remove(session);
        };

        listener.NetworkReceiveEvent += (peer, reader, _, _) =>
        {
            var session = WorldHost.Current.Sessions.Find(x => x.Connection == peer);
            PacketDispatcher.Dispatch(session, reader);
            reader.Recycle();
        };

        Device.Start(Config.Port);
    }

    public void HandleData() => Device.PollEvents();
}
