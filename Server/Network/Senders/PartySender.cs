using CryBits.Enums;
using CryBits.Server.Entities;
using LiteNetLib.Utils;

namespace CryBits.Server.Network.Senders;

internal static class PartySender
{
    public static void Party(Player player)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.Party);
        data.Put((byte)player.Party.Count);
        for (byte i = 0; i < player.Party.Count; i++) data.Put(player.Party[i].Name);
        Send.ToPlayer(player, data);
    }

    public static void PartyInvitation(Player player, string playerInvitation)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.PartyInvitation);
        data.Put(playerInvitation);
        Send.ToPlayer(player, data);
    }
}
