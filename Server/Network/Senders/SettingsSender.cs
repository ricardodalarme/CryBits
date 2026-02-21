using CryBits.Enums;
using CryBits.Packets.Server;
using CryBits.Server.Entities;
using static CryBits.Globals;

namespace CryBits.Server.Network.Senders;

internal static class SettingsSender
{
    public static void ServerData(Account account)
    {
        Send.ToPlayer(account, ServerPacket.ServerData, new ServerDataPacket { Config = Config });
    }
}
