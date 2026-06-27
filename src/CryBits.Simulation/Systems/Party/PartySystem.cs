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
        var appearance = e.Get<PlayerAppearance>()!;
        var party = world.AddOrGet<PartyState>(entityId);
        if (party == null) return;

        var invitedId = world.FindPlayer(targetName);

        if (invitedId == null)
        {
            tick.Events.Emit(new ChatMessageEvent { RecipientId = entityId, Text = "The player isn't connected.", ColorArgb = ChatColors.White });
            return;
        }

        if (invitedId.Value == entityId)
        {
            tick.Events.Emit(new ChatMessageEvent { RecipientId = entityId, Text = "You can't be invited.", ColorArgb = ChatColors.White });
            return;
        }

        var invitedE = world.Entities.Get(invitedId.Value);
        if (invitedE == null) return;
        var invitedParty = invitedE.Get<PartyState>();

        if (invitedParty?.Members.Count > 0)
        {
            tick.Events.Emit(new ChatMessageEvent { RecipientId = entityId, Text = "The player is already part of a party.", ColorArgb = ChatColors.White });
            return;
        }

        if (invitedParty?.PendingInviterId.HasValue == true)
        {
            tick.Events.Emit(new ChatMessageEvent { RecipientId = entityId, Text = "The player is analyzing an invitation to another party.", ColorArgb = ChatColors.White });
            return;
        }

        if (party.Members.Count == Config.MaxPartyMembers - 1)
        {
            tick.Events.Emit(new ChatMessageEvent { RecipientId = entityId, Text = "Your party is full.", ColorArgb = ChatColors.White });
            return;
        }

        var targetParty = world.AddOrGet<PartyState>(invitedId.Value);
        if (targetParty == null) return;
        targetParty.PendingInviterId = entityId;
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
            tick.Events.Emit(new ChatMessageEvent { RecipientId = entityId, Text = "You are already part of a party.", ColorArgb = ChatColors.White });
            return;
        }

        if (inviterId == null)
        {
            tick.Events.Emit(new ChatMessageEvent { RecipientId = entityId, Text = "Who invited you is no longer available.", ColorArgb = ChatColors.White });
            return;
        }

        var inviterE = world.Entities.Get(inviterId.Value);
        if (inviterE == null) return;
        var inviterParty = inviterE.Get<PartyState>();
        if (inviterParty == null) return;
        var inviterAppearance = inviterE.Get<PlayerAppearance>()!;

        if (inviterParty.Members.Count == Config.MaxPartyMembers - 1)
        {
            tick.Events.Emit(new ChatMessageEvent { RecipientId = entityId, Text = "The party is full.", ColorArgb = ChatColors.White });
            return;
        }

        for (byte i = 0; i < inviterParty.Members.Count; i++)
        {
            var memberId = inviterParty.Members[i];
            var memberParty = world.Get<PartyState>(memberId);
            if (memberParty == null) continue;
            memberParty.Members.Add(entityId);
            world.MarkDirty<PartyState>(memberId);
            if (memberId != inviterId.Value)
                party.Members.Add(memberId);
        }

        party.Members.Insert(0, inviterId.Value);
        inviterParty.Members.Add(entityId);
        party.PendingInviterId = null;
        tick.Events.Emit(new ChatMessageEvent { RecipientId = entityId, Text = "You joined " + inviterAppearance.Name + "'s party.", ColorArgb = ChatColors.White });
        tick.Events.Emit(new ChatMessageEvent { RecipientId = inviterId.Value, Text = appearance.Name + " joined the party.", ColorArgb = ChatColors.White });
        world.MarkDirty<PartyState>(entityId);
        world.MarkDirty<PartyState>(inviterId.Value);
        for (byte i = 0; i < party.Members.Count; i++) world.MarkDirty<PartyState>(party.Members[i]);
    }

    private void Decline(World world, Tick tick, EntityId entityId)
    {
        var e = world.Entities.Get(entityId);
        if (e == null) return;
        var appearance = e.Get<PlayerAppearance>()!;
        var party = e.Get<PartyState>();
        if (party == null) return;

        var inviterId = party.PendingInviterId;
        if (inviterId != null) tick.Events.Emit(new ChatMessageEvent { RecipientId = inviterId.Value, Text = appearance.Name + " decline the party.", ColorArgb = ChatColors.White });
        world.Remove<PartyState>(entityId);
    }

    private void Leave(World world, EntityId entityId)
    {
        var party = world.Get<PartyState>(entityId);
        if (party == null || party.Members.Count == 0) return;

        for (byte i = 0; i < party.Members.Count; i++)
        {
            var memberParty = world.Get<PartyState>(party.Members[i]);
            if (memberParty == null) continue;
            memberParty.Members.Remove(entityId);
            world.MarkDirty<PartyState>(party.Members[i]);
        }

        world.Remove<PartyState>(entityId);
    }
}
