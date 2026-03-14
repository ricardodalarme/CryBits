using CryBits.Client.Components.Party;
using CryBits.Client.Framework;
using CryBits.Client.Network.Senders;
using CryBits.Client.UI.Game.Views;
using CryBits.Client.Worlds;
using CryBits.Packets.Server;

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

        // Ensure the component exists before writing into it.
        if (!world.Has<PartyComponent>(entity))
            world.Add(entity, new PartyComponent());

        ref var party = ref context.LocalPlayer.GetParty();
        party.Members = new Arch.Core.Entity[packet.MemberIds.Length];
        for (byte i = 0; i < party.Members.Length; i++)
            party.Members[i] = context.GetNetworkEntity(packet.MemberIds[i]);
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
