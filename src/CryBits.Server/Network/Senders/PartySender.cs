using CryBits.Network.Packets.Server;
using CryBits.Simulation.Components;
using System;
using CryBits.Simulation.State;
using CryBits.Server.Core;

namespace CryBits.Server.Network.Senders;

internal sealed class PartySender(PackageSender packageSender)
{
    public static PartySender Instance { get; } = new(PackageSender.Instance);

    public void Party(EntityId entityId)
    {
        var party = WorldHost.Current.Entities.Get(entityId)!.Get<PartyState>()!;
        var packet = new PartyPacket { MemberIds = new Guid[party.Members.Count] };
        for (var i = 0; i < party.Members.Count; i++) packet.MemberIds[i] = party.Members[i].Value;
        packageSender.ToPlayer(entityId, packet);
    }

    public void PartyInvitation(EntityId entityId, string playerInvitation)
    {
        packageSender.ToPlayer(entityId, new PartyInvitationPacket { PlayerInvitation = playerInvitation });
    }
}
