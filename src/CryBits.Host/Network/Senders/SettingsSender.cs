using CryBits.Network.Packets.Server;
using CryBits.Host.Core;
using static CryBits.Definitions.Globals;

namespace CryBits.Host.Network.Senders;

internal sealed class SettingsSender(PackageSender packageSender)
{
    public static SettingsSender Instance { get; } = new(PackageSender.Instance);

    public void ServerData(GameSession session)
    {
        packageSender.ToPlayer(session, new ServerDataPacket { Config = Config });
    }
}
