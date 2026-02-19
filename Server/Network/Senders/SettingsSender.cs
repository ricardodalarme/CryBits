using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Server.Entities;
using LiteNetLib.Utils;
using static CryBits.Globals;

namespace CryBits.Server.Network.Senders;

internal static class SettingsSender
{
    public static void ServerData(Account account)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.ServerData);
        data.WriteObject(Config);
        Send.ToPlayer(account, data);
    }
}