using System.Drawing;
using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;
using LiteNetLib.Utils;
using static CryBits.Globals;

namespace CryBits.Server.Network.Handlers;

internal static class PartyHandler
{
    internal static void PartyInvite(Player player, NetDataReader data)
    {
        var name = data.GetString();

        // Encontra o jogador
        var invited = Player.Find(name);

        // Verifica se o jogador está convectado
        if (invited == null)
        {
            ChatSender.Message(player, "The player isn't connected.", Color.White);
            return;
        }

        // Verifica se não está tentando se convidar
        if (invited == player)
        {
            ChatSender.Message(player, "You can't be invited.", Color.White);
            return;
        }

        // Verifica se já tem um grupo
        if (invited.Party.Count != 0)
        {
            ChatSender.Message(player, "The player is already part of a party.", Color.White);
            return;
        }

        // Verifica se o jogador já está analisando um convite para algum grupo
        if (!string.IsNullOrEmpty(invited.PartyRequest))
        {
            ChatSender.Message(player, "The player is analyzing an invitation to another party.", Color.White);
            return;
        }

        // Verifica se o grupo está cheio
        if (player.Party.Count == MaxPartyMembers - 1)
        {
            ChatSender.Message(player, "Your party is full.", Color.White);
            return;
        }

        // Convida o jogador
        invited.PartyRequest = player.Name;
        PartySender.PartyInvitation(invited, player.Name);
    }

    internal static void PartyAccept(Player player)
    {
        var invitation = Player.Find(player.PartyRequest);

        // Verifica se já tem um grupo
        if (player.Party.Count != 0)
        {
            ChatSender.Message(player, "You are already part of a party.", Color.White);
            return;
        }

        // Verifica se quem chamou ainda está disponível
        if (invitation == null)
        {
            ChatSender.Message(player, "Who invited you is no longer available.", Color.White);
            return;
        }

        // Verifica se o grupo está cheio
        if (invitation.Party.Count == MaxPartyMembers - 1)
        {
            ChatSender.Message(player, "The party is full.", Color.White);
            return;
        }

        // Entra na festa
        for (byte i = 0; i < invitation.Party.Count; i++)
        {
            invitation.Party[i].Party.Add(player);
            player.Party.Add(invitation.Party[i]);
        }

        player.Party.Insert(0, invitation);
        invitation.Party.Add(player);
        player.PartyRequest = string.Empty;
        ChatSender.Message(invitation, player.Name + " joined the party.", Color.White);

        // Envia os dados para o grupo
        PartySender.Party(player);
        for (byte i = 0; i < player.Party.Count; i++) PartySender.Party(player.Party[i]);
    }

    internal static void PartyDecline(Player player)
    {
        var invitation = Player.Find(player.PartyRequest);

        // Recusa o convite
        if (invitation != null) ChatSender.Message(invitation, player.Name + " decline the party.", Color.White);
        player.PartyRequest = string.Empty;
    }

    internal static void PartyLeave(Player player)
    {
        // Sai do grupo
        player.PartyLeave();
    }
}
