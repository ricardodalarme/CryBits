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
        // Lê os dados do grupo
        Player.Me.Party = new Player[data.GetByte()];
        for (byte i = 0; i < Player.Me.Party.Length; i++) Player.Me.Party[i] = Player.Get(data.GetString());
    }

    internal static void PartyInvitation(NetDataReader data)
    {
        // Nega o pedido caso o jogador não quiser receber convites
        if (!Options.Party)
        {
            PartySender.PartyDecline();
            return;
        }

        // Abre a janela de convite para o grupo
        PanelsEvents.PartyInvitation = data.GetString();
        Panels.PartyInvitation.Visible = true;
    }
}
