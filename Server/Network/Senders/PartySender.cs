using CryBits.Packets.Server;
using CryBits.Server.ECS;
using CryBits.Server.ECS.Components;
using CryBits.Server.Entities;
using CryBits.Server.World;

namespace CryBits.Server.Network.Senders;

internal static class PartySender
{
    private static CryBits.Server.ECS.Core.World World => ServerContext.Instance.World;

    public static void Party(Player player)
    {
        var party = World.Get<PartyComponent>(player.EntityId);
        var packet = new PartyPacket { Members = new string[party.MemberEntityIds.Count] };
        for (int i = 0; i < party.MemberEntityIds.Count; i++)
        {
            var memberId = party.MemberEntityIds[i];
            packet.Members[i] = World.Get<PlayerDataComponent>(memberId).Name;
        }
        Send.ToPlayer(player, packet);
    }

    public static void PartyInvitation(Player player, string playerInvitation)
    {
        Send.ToPlayer(player, new PartyInvitationPacket { PlayerInvitation = playerInvitation });
    }
}
