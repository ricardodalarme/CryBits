using CryBits.Client.Framework;
using CryBits.Client.Network.Senders;
using CryBits.Client.UI.Game.Views;
using CryBits.Client.Worlds;
using CryBits.Protocol;
using CryBits.Protocol.Packets.Server;
using CryBits.Simulation.Components;
using CryBits.Simulation.State;

namespace CryBits.Client.Network.Handlers;

internal class PartyHandler(PartySender partySender, GameContext context)
{
    [PacketHandler]
    internal void Party(PartyPacket packet)
    {
        var entityNullable = context.LocalPlayer.Entity;
        if (entityNullable is null) return;
        var entity = entityNullable.Value;
        var world = context.World;

        if (packet.MemberIds.Length == 0)
        {
            // No members — party disbanded or player left; drop the component.
            if (world.Has<PartyState>(entity))
                world.Remove<PartyState>(entity);
            return;
        }

        var party = world.AddOrGet<PartyState>(entity);
        if (party is null) return;
        party.Members.Clear();
        for (byte i = 0; i < packet.MemberIds.Length; i++)
            party.Members.Add(new EntityId(packet.MemberIds[i]));
    }

    [PacketHandler]
    internal void PartyInvitation(PartyInvitationPacket packet)
    {
        // Decline if player disabled party invites
        if (!Options.Instance.Party)
        {
            partySender.PartyDecline();
            return;
        }

        // Show party invitation panel
        PartyInvitationView.Show(packet.PlayerInvitation);
    }
}
