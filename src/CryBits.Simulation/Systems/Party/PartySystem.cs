using CryBits.Simulation.Components;
using CryBits.Definitions.Utils;
using CryBits.Simulation.Events;
using static CryBits.Definitions.Globals;
using CryBits.Simulation.Core;
using CryBits.Simulation.Intents;
using CryBits.Simulation.State;

namespace CryBits.Simulation.Systems.Party;

public sealed class PartySystem : ISimulationSystem
{
    public void Execute(World world, Tick tick)
    {
        foreach (var intent in tick.Intents.All)
        {
            switch (intent)
            {
                case PartyInviteIntent i: Invite(world, tick, i.SourceEntityId, i.PlayerName); break;
                case PartyAcceptIntent a: Accept(world, tick, a.SourceEntityId); break;
                case PartyDeclineIntent d: Decline(world, tick, d.SourceEntityId); break;
                case PartyLeaveIntent l: Leave(world, l.SourceEntityId); break;
            }
        }

        foreach (var ev in tick.Events.Events)
        {
            if (ev is not PlayerDisconnectedEvent e) continue;
            var playerId = world.FindPlayer(e.PlayerId);
            if (playerId != null) Leave(world, playerId.Value);
        }
    }

    private void Invite(World world, Tick tick, EntityId entityId, string targetName)
    {
        var e = world.Entities.Get(entityId);
        if (e == null) return;
        var party = e.Get<PartyState>();

        var invitedId = world.FindPlayer(targetName);

        if (invitedId == null)
        {
            tick.Events.Emit(new ChatMessageEvent(tick.TickNumber, entityId, "The player isn't connected.", ChatColors.White));
            return;
        }

        if (invitedId.Value == entityId)
        {
            tick.Events.Emit(new ChatMessageEvent(tick.TickNumber, entityId, "You can't be invited.", ChatColors.White));
            return;
        }

        var invitedE = world.Entities.Get(invitedId.Value);
        if (invitedE == null) return;
        var invitedParty = invitedE.Get<PartyState>();

        if (invitedParty?.Members.Count > 0)
        {
            tick.Events.Emit(new ChatMessageEvent(tick.TickNumber, entityId, "The player is already part of a party.", ChatColors.White));
            return;
        }

        if (invitedParty?.PendingInviterId.HasValue == true)
        {
            tick.Events.Emit(new ChatMessageEvent(tick.TickNumber, entityId, "The player is analyzing an invitation to another party.", ChatColors.White));
            return;
        }

        if (party != null && party.Members.Count == Config.MaxPartyMembers - 1)
        {
            tick.Events.Emit(new ChatMessageEvent(tick.TickNumber, entityId, "Your party is full.", ChatColors.White));
            return;
        }

        world.Set(invitedId.Value, new PartyState([], entityId));
    }

    private void Accept(World world, Tick tick, EntityId entityId)
    {
        var e = world.Entities.Get(entityId);
        if (e == null) return;
        var appearance = e.Get<PlayerAppearance>()!;
        var party = e.Get<PartyState>();
        if (party == null) return;

        var inviterId = party.PendingInviterId;

        if (party.Members.Count != 0)
        {
            tick.Events.Emit(new ChatMessageEvent(tick.TickNumber, entityId, "You are already part of a party.", ChatColors.White));
            return;
        }

        if (inviterId == null)
        {
            tick.Events.Emit(new ChatMessageEvent(tick.TickNumber, entityId, "Who invited you is no longer available.", ChatColors.White));
            return;
        }

        var inviterE = world.Entities.Get(inviterId.Value);
        if (inviterE == null) return;
        var inviterParty = inviterE.Get<PartyState>();
        if (inviterParty == null) return;
        var inviterAppearance = inviterE.Get<PlayerAppearance>()!;

        if (inviterParty.Members.Count == Config.MaxPartyMembers - 1)
        {
            tick.Events.Emit(new ChatMessageEvent(tick.TickNumber, entityId, "The party is full.", ChatColors.White));
            return;
        }

        // Build new member lists
        var newInviterMembers = new List<EntityId>(inviterParty.Members) { entityId };
        var newMyMembers = new List<EntityId> { inviterId.Value };
        newMyMembers.AddRange(inviterParty.Members);

        // Update all members' PartyState
        world.Set(entityId, new PartyState(newMyMembers, null));
        world.Set(inviterId.Value, new PartyState(newInviterMembers, inviterParty.PendingInviterId));
        foreach (var memberId in inviterParty.Members)
        {
            var memberParty = world.Get<PartyState>(memberId);
            if (memberParty == null) continue;
            var newMemberMembers = new List<EntityId>(memberParty.Members) { entityId };
            world.Set(memberId, new PartyState(newMemberMembers, memberParty.PendingInviterId));
        }

        tick.Events.Emit(new ChatMessageEvent(tick.TickNumber, entityId, "You joined " + inviterAppearance.Name + "'s party.", ChatColors.White));
        tick.Events.Emit(new ChatMessageEvent(tick.TickNumber, inviterId.Value, appearance.Name + " joined the party.", ChatColors.White));
    }

    private void Decline(World world, Tick tick, EntityId entityId)
    {
        var e = world.Entities.Get(entityId);
        if (e == null) return;
        var appearance = e.Get<PlayerAppearance>()!;
        var party = e.Get<PartyState>();
        if (party == null) return;

        var inviterId = party.PendingInviterId;
        if (inviterId != null) tick.Events.Emit(new ChatMessageEvent(tick.TickNumber, inviterId.Value, appearance.Name + " decline the party.", ChatColors.White));
        world.Remove<PartyState>(entityId);
    }

    private void Leave(World world, EntityId entityId)
    {
        var party = world.Get<PartyState>(entityId);
        if (party == null || party.Members.Count == 0) return;

        foreach (var memberId in party.Members)
        {
            var memberParty = world.Get<PartyState>(memberId);
            if (memberParty == null) continue;
            var newMembers = new List<EntityId>(memberParty.Members);
            newMembers.Remove(entityId);
            world.Set(memberId, new PartyState(newMembers, memberParty.PendingInviterId));
        }

        world.Remove<PartyState>(entityId);
    }
}
