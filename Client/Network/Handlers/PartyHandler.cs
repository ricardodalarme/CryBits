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
        ref var party = ref context.LocalPlayer.GetParty();
        party.Members = new Arch.Core.Entity[packet.Members.Length];
        for (byte i = 0; i < party.Members.Length; i++)
            party.Members[i] = context.GetPlayerEntity(packet.Members[i]);
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
