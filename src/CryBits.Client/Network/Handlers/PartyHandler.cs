using CryBits.Client.Components.Party;
using CryBits.Client.Framework;
using CryBits.Client.Network.Senders;
using CryBits.Client.UI.Game.Views;
using CryBits.Client.Worlds;
using CryBits.Packets.Server;
using System;

namespace CryBits.Client.Network.Handlers;

internal class PartyHandler(PartySender partySender, GameContext context)
{
    [PacketHandler]
    internal void Party(PartyPacket packet)
    {
        var entity = context.LocalPlayer.Entity;
        var world = context.World;

        if (packet.MemberIds.Length == 0)
        {
            // No members — party disbanded or player left; drop the component.
            if (world.Has<PartyComponent>(entity))
                world.Remove<PartyComponent>(entity);
            return;
        }

        ref var party = ref world.AddOrGet(entity, new PartyComponent());
        party.MemberIds = new Guid[packet.MemberIds.Length];
        for (byte i = 0; i < party.MemberIds.Length; i++)
            party.MemberIds[i] = packet.MemberIds[i];
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
