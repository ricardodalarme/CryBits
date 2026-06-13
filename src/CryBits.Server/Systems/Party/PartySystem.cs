using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;
using CryBits.Server.Simulation.Core;
using CryBits.Simulation.Events;
using CryBits.Server.World;
using System.Drawing;
using static CryBits.Globals;
using CryBits.Simulation.Core;

namespace CryBits.Server.Systems.Party;

internal sealed class PartySystem(ChatSender chatSender, PartySender partySender) : ISimulationSystem
{
    public static PartySystem Instance { get; } = new(ChatSender.Instance, PartySender.Instance);

    internal void Invite(Player player, string targetName)
    {
        var invited = Player.Find(targetName);

        if (invited == null)
        {
            chatSender.Message(player, "The player isn't connected.", Color.White);
            return;
        }

        if (invited == player)
        {
            chatSender.Message(player, "You can't be invited.", Color.White);
            return;
        }

        if (invited.Party.Count != 0)
        {
            chatSender.Message(player, "The player is already part of a party.", Color.White);
            return;
        }

        if (!string.IsNullOrEmpty(invited.PartyRequest))
        {
            chatSender.Message(player, "The player is analyzing an invitation to another party.", Color.White);
            return;
        }

        if (player.Party.Count == Config.MaxPartyMembers - 1)
        {
            chatSender.Message(player, "Your party is full.", Color.White);
            return;
        }

        invited.PartyRequest = player.Name;
        partySender.PartyInvitation(invited, player.Name);
    }

    internal void Accept(Player player)
    {
        var invitation = Player.Find(player.PartyRequest);

        if (player.Party.Count != 0)
        {
            chatSender.Message(player, "You are already part of a party.", Color.White);
            return;
        }

        if (invitation == null)
        {
            chatSender.Message(player, "Who invited you is no longer available.", Color.White);
            return;
        }

        if (invitation.Party.Count == Config.MaxPartyMembers - 1)
        {
            chatSender.Message(player, "The party is full.", Color.White);
            return;
        }

        for (byte i = 0; i < invitation.Party.Count; i++)
        {
            invitation.Party[i].Party.Add(player);
            player.Party.Add(invitation.Party[i]);
        }

        player.Party.Insert(0, invitation);
        invitation.Party.Add(player);
        player.PartyRequest = string.Empty;
        chatSender.Message(invitation, player.Name + " joined the party.", Color.White);

        partySender.Party(player);
        for (byte i = 0; i < player.Party.Count; i++) partySender.Party(player.Party[i]);
    }

    internal void Decline(Player player)
    {
        var invitation = Player.Find(player.PartyRequest);
        if (invitation != null) chatSender.Message(invitation, player.Name + " decline the party.", Color.White);
        player.PartyRequest = string.Empty;
    }

    public void Leave(Player player)
    {
        if (player.Party.Count == 0) return;

        for (byte i = 0; i < player.Party.Count; i++)
            player.Party[i].Party.Remove(player);

        for (byte i = 0; i < player.Party.Count; i++)
            partySender.Party(player.Party[i]);

        player.Party.Clear();
        partySender.Party(player);
    }

    public void Execute(GameWorld world, Tick tick)
    {
        foreach (var ev in tick.Events.Events)
        {
            if (ev is not PlayerDisconnectedEvent e) continue;
            var player = world.FindPlayer(e.PlayerId);
            if (player != null) Leave(player);
        }
    }
}
