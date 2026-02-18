using CryBits.Enums;
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
        data.Put(GameName);
        data.Put(WelcomeMessage);
        data.Put(Port);
        data.Put(MaxPlayers);
        data.Put(MaxCharacters);
        data.Put(MaxPartyMembers);
        data.Put(MaxMapItems);
        data.Put(NumPoints);
        data.Put(MinNameLength);
        data.Put(MaxNameLength);
        data.Put(MinPasswordLength);
        data.Put(MaxPasswordLength);
        Send.ToPlayer(account, data);
    }
}