using CryBits.Packets.Server;
using CryBits.Server.World;
using static CryBits.Globals;

namespace CryBits.Server.Network.Senders;

internal static class SettingsSender
{
    public static void ServerData(GameSession session)
    {
        Send.ToPlayer(session, new ServerDataPacket { Config = Config });
    }
}
