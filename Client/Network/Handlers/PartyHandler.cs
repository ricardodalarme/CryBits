using CryBits.Client.Entities;
using CryBits.Client.Framework;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Network.Senders;
using CryBits.Client.UI.Events;
using CryBits.Enums;
using CryBits.Packets.Server;

namespace CryBits.Client.Network.Handlers;

internal static class PartyHandler
{
    [PacketHandler(ServerPacket.Party)]
    internal static void Party(PartyPacket packet)
    {
        // Read party members
        Player.Me.Party = new Player[packet.Members.Length];
        for (byte i = 0; i < Player.Me.Party.Length; i++) Player.Me.Party[i] = Player.Get(packet.Members[i]);
    }

    [PacketHandler(ServerPacket.PartyInvitation)]
    internal static void PartyInvitation(PartyInvitationPacket packet)
    {
        // Decline if player disabled party invites
        if (!Options.Party)
        {
            PartySender.PartyDecline();
            return;
        }

        // Show party invitation panel
        PanelsEvents.PartyInvitation = packet.PlayerInvitation;
        Panels.PartyInvitation.Visible = true;
    }
}
