using CryBits.Enums;
using CryBits.Packets.Server;
using CryBits.Server.Entities;

namespace CryBits.Server.Network.Senders;

internal static class PartySender
{
    public static void Party(Player player)
    {
        var packet = new PartyPacket { Members = new string[player.Party.Count] };
        for (int i = 0; i < player.Party.Count; i++) packet.Members[i] = player.Party[i].Name;
        Send.ToPlayer(player, ServerPacket.Party, packet);
    }

    public static void PartyInvitation(Player player, string playerInvitation)
    {
        Send.ToPlayer(player, ServerPacket.PartyInvitation, new PartyInvitationPacket { PlayerInvitation = playerInvitation });
    }
}
