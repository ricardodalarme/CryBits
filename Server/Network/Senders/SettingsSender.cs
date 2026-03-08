using CryBits.Packets.Server;
using CryBits.Server.World;
using static CryBits.Globals;

namespace CryBits.Server.Network.Senders;

internal sealed class SettingsSender(PackageSender packageSender)
{
    public static SettingsSender Instance { get; } = new(PackageSender.Instance);

    private readonly PackageSender _packageSender = packageSender;

    public void ServerData(GameSession session)
    {
        _packageSender.ToPlayer(session, new ServerDataPacket { Config = Config });
    }
}
