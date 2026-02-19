using CryBits.Client.Entities;
using CryBits.Client.Framework;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Network.Senders;
using CryBits.Client.UI.Events;
using LiteNetLib.Utils;

namespace CryBits.Client.Network.Handlers;

internal static class PartyHandler
{
    internal static void Party(NetDataReader data)
    {
        // Read party members
        Player.Me.Party = new Player[data.GetByte()];
        for (byte i = 0; i < Player.Me.Party.Length; i++) Player.Me.Party[i] = Player.Get(data.GetString());
    }

    internal static void PartyInvitation(NetDataReader data)
    {
        // Decline if player disabled party invites
        if (!Options.Party)
        {
            PartySender.PartyDecline();
            return;
        }

        // Show party invitation panel
        PanelsEvents.PartyInvitation = data.GetString();
        Panels.PartyInvitation.Visible = true;
    }
}
