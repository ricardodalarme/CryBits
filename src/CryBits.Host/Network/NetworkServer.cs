using CryBits.Host.Core;
using CryBits.Host.Services;
using LiteNetLib;
using static CryBits.Definitions.Globals;

namespace CryBits.Host.Network;

internal sealed class NetworkServer(CharacterService characterService)
{
    public static NetworkServer Instance { get; } = new(CharacterService.Instance);

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

        listener.PeerConnectedEvent += peer => WorldHost.Current.Sessions.Add(new Session(peer));

        listener.PeerDisconnectedEvent += (peer, _) =>
        {
            var session = WorldHost.Current.Sessions.Find(x => x.Connection == peer);
            if (session == null) return;
            if (session.Character is { } characterId) characterService.Leave(characterId);
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
