using CryBits.Client.Entities;
using CryBits.Client.Framework;
using CryBits.Client.Network.Senders;
using CryBits.Client.UI;
using CryBits.Client.UI.Game.Views;
using CryBits.Packets.Server;

namespace CryBits.Client.Network.Handlers;

internal class PartyHandler(PartySender partySender)
{
    [PacketHandler]
    internal void Party(PartyPacket packet)
    {
        // Read party members
        Player.Me.Party = new Player[packet.Members.Length];
        for (byte i = 0; i < Player.Me.Party.Length; i++) Player.Me.Party[i] = Player.Get(packet.Members[i]);
    }

    [PacketHandler]
    internal void PartyInvitation(PartyInvitationPacket packet)
    {
        // Decline if player disabled party invites
        if (!Options.Party)
        {
            partySender.PartyDecline();
            return;
        }

        // Show party invitation panel
        PartyInvitationView.Show(packet.PlayerInvitation);
    }
}
