using CryBits.Network.Packets.Server;
using CryBits.Server.Simulation.State;
using CryBits.Server.Simulation.State.Components;
using CryBits.Server.World;
using System;

namespace CryBits.Server.Network.Senders;

internal sealed class PartySender(PackageSender packageSender)
{
    public static PartySender Instance { get; } = new(PackageSender.Instance);

    public void Party(EntityId entityId)
    {
        var party = GameWorld.Current.Entities.Get(entityId)!.Get<PartyState>()!;
        var packet = new PartyPacket { MemberIds = new Guid[party.Members.Count] };
        for (var i = 0; i < party.Members.Count; i++) packet.MemberIds[i] = party.Members[i].Value;
        packageSender.ToPlayer(entityId, packet);
    }

    public void PartyInvitation(EntityId entityId, string playerInvitation)
    {
        packageSender.ToPlayer(entityId, new PartyInvitationPacket { PlayerInvitation = playerInvitation });
    }
}
