using System.Drawing;
using CryBits.Server.ECS;
using CryBits.Server.ECS.Components;
using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;
using CryBits.Server.World;
using static CryBits.Globals;

namespace CryBits.Server.Systems;

/// <summary>Owns all party lifecycle logic.</summary>
internal static class PartySystem
{
    private static CryBits.Server.ECS.Core.World World => ServerContext.Instance.World;

    private static Player? PlayerFromEntityId(int entityId)
    {
        var session = GameWorld.Current.Sessions
            .Find(s => s.IsPlaying && s.Character?.EntityId == entityId);
        return session?.Character;
    }

    /// <summary>Sends a party invitation from <paramref name="player"/> to the named target.</summary>
    internal static void Invite(Player player, string targetName)
    {
        var invited = Player.Find(targetName);

        if (invited == null)
        {
            ChatSender.Message(player, "The player isn't connected.", Color.White);
            return;
        }

        if (invited == player)
        {
            ChatSender.Message(player, "You can't be invited.", Color.White);
            return;
        }

        var invitedParty = World.Get<PartyComponent>(invited.EntityId);
        if (invitedParty.MemberEntityIds.Count != 0)
        {
            ChatSender.Message(player, "The player is already part of a party.", Color.White);
            return;
        }

        if (!string.IsNullOrEmpty(invitedParty.PendingRequest))
        {
            ChatSender.Message(player, "The player is analyzing an invitation to another party.", Color.White);
            return;
        }

        var playerParty = World.Get<PartyComponent>(player.EntityId);
        if (playerParty.MemberEntityIds.Count == Config.MaxPartyMembers - 1)
        {
            ChatSender.Message(player, "Your party is full.", Color.White);
            return;
        }

        invitedParty.PendingRequest = player.Get<PlayerDataComponent>().Name;
        PartySender.PartyInvitation(invited, player.Get<PlayerDataComponent>().Name);
    }

    /// <summary>Accepts the pending party invitation for <paramref name="player"/>.</summary>
    internal static void Accept(Player player)
    {
        var party      = World.Get<PartyComponent>(player.EntityId);
        var invitation = Player.Find(party.PendingRequest);

        if (party.MemberEntityIds.Count != 0)
        {
            ChatSender.Message(player, "You are already part of a party.", Color.White);
            return;
        }

        if (invitation == null)
        {
            ChatSender.Message(player, "Who invited you is no longer available.", Color.White);
            return;
        }

        var invitationParty = World.Get<PartyComponent>(invitation.EntityId);
        if (invitationParty.MemberEntityIds.Count == Config.MaxPartyMembers - 1)
        {
            ChatSender.Message(player, "The party is full.", Color.White);
            return;
        }

        // Add the new player to every existing member's list, and add each member to player's list
        foreach (var memberId in invitationParty.MemberEntityIds)
        {
            var memberParty = World.Get<PartyComponent>(memberId);
            memberParty.MemberEntityIds.Add(player.EntityId);
            party.MemberEntityIds.Add(memberId);
        }

        party.MemberEntityIds.Insert(0, invitation.EntityId);
        invitationParty.MemberEntityIds.Add(player.EntityId);
        party.PendingRequest = string.Empty;

        ChatSender.Message(invitation, player.Get<PlayerDataComponent>().Name + " joined the party.", Color.White);

        PartySender.Party(player);
        foreach (var memberId in party.MemberEntityIds)
        {
            var member = PlayerFromEntityId(memberId);
            if (member != null) PartySender.Party(member);
        }
    }

    /// <summary>Declines the pending party invitation for <paramref name="player"/>.</summary>
    internal static void Decline(Player player)
    {
        var party      = World.Get<PartyComponent>(player.EntityId);
        var invitation = Player.Find(party.PendingRequest);
        if (invitation != null)
            ChatSender.Message(invitation, player.Get<PlayerDataComponent>().Name + " decline the party.", Color.White);
        party.PendingRequest = string.Empty;
    }

    /// <summary>
    /// Removes <paramref name="player"/> from their party, notifies all remaining members.
    /// </summary>
    public static void Leave(Player player)
    {
        var party = World.Get<PartyComponent>(player.EntityId);
        if (party.MemberEntityIds.Count == 0) return;

        foreach (var memberId in party.MemberEntityIds)
        {
            var memberParty = World.Get<PartyComponent>(memberId);
            memberParty.MemberEntityIds.Remove(player.EntityId);
        }

        foreach (var memberId in party.MemberEntityIds)
        {
            var member = PlayerFromEntityId(memberId);
            if (member != null) PartySender.Party(member);
        }

        party.MemberEntityIds.Clear();
        PartySender.Party(player);
    }
}
