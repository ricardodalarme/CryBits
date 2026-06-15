using CryBits.Transport.Packets.Server;
using CryBits.Simulation.Components;
using CryBits.Simulation.State;

namespace CryBits.Host.Network.Senders;

internal sealed class PartySender(PackageSender packageSender, EntityRegistry entities)
{
    public void Party(EntityId entityId)
    {
        var party = entities.Get(entityId)!.Get<PartyState>()!;
        var packet = new PartyPacket { MemberIds = new long[party.Members.Count] };
        for (var i = 0; i < party.Members.Count; i++) packet.MemberIds[i] = party.Members[i].Value;
        packageSender.ToPlayer(entityId, packet);
    }

    public void PartyInvitation(EntityId entityId, string playerInvitation)
    {
        packageSender.ToPlayer(entityId, new PartyInvitationPacket { PlayerInvitation = playerInvitation });
    }
}
