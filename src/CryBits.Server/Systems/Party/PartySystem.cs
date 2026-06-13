using CryBits.Server.Simulation.Core;
using CryBits.Simulation.Components;
using CryBits.Simulation.Events;
using CryBits.Server.World;
using System.Drawing;
using static CryBits.Definitions.Globals;
using CryBits.Simulation.Core;
using CryBits.Simulation.State;

namespace CryBits.Server.Systems.Party;

internal sealed class PartySystem : ISimulationSystem
{
    public static PartySystem Instance { get; } = new();

    internal void Invite(EntityId entityId, string targetName)
    {
        var world = GameWorld.Current;
        var e = world.Entities.Get(entityId)!;
        var appearance = e.Get<PlayerAppearance>()!;
        var party = e.Get<PartyState>()!;

        var invitedId = world.FindPlayer(targetName);

        if (invitedId == null)
        {
            world.CurrentTick?.Events.Emit(new ChatMessageEvent { RecipientId = entityId.Value, Text = "The player isn't connected.", ColorArgb = Color.White.ToArgb() });
            return;
        }

        if (invitedId.Value == entityId)
        {
            world.CurrentTick?.Events.Emit(new ChatMessageEvent { RecipientId = entityId.Value, Text = "You can't be invited.", ColorArgb = Color.White.ToArgb() });
            return;
        }

        var invitedE = world.Entities.Get(invitedId.Value)!;
        var invitedParty = invitedE.Get<PartyState>()!;

        if (invitedParty.Members.Count != 0)
        {
            world.CurrentTick?.Events.Emit(new ChatMessageEvent { RecipientId = entityId.Value, Text = "The player is already part of a party.", ColorArgb = Color.White.ToArgb() });
            return;
        }

        if (!string.IsNullOrEmpty(invitedParty.Request))
        {
            world.CurrentTick?.Events.Emit(new ChatMessageEvent { RecipientId = entityId.Value, Text = "The player is analyzing an invitation to another party.", ColorArgb = Color.White.ToArgb() });
            return;
        }

        if (party.Members.Count == Config.MaxPartyMembers - 1)
        {
            world.CurrentTick?.Events.Emit(new ChatMessageEvent { RecipientId = entityId.Value, Text = "Your party is full.", ColorArgb = Color.White.ToArgb() });
            return;
        }

        invitedParty.Request = appearance.Name;
    }

    internal void Accept(EntityId entityId)
    {
        var world = GameWorld.Current;
        var e = world.Entities.Get(entityId)!;
        var appearance = e.Get<PlayerAppearance>()!;
        var party = e.Get<PartyState>()!;

        var inviterId = world.FindPlayer(party.Request);

        if (party.Members.Count != 0)
        {
            world.CurrentTick?.Events.Emit(new ChatMessageEvent { RecipientId = entityId.Value, Text = "You are already part of a party.", ColorArgb = Color.White.ToArgb() });
            return;
        }

        if (inviterId == null)
        {
            world.CurrentTick?.Events.Emit(new ChatMessageEvent { RecipientId = entityId.Value, Text = "Who invited you is no longer available.", ColorArgb = Color.White.ToArgb() });
            return;
        }

        var inviterE = world.Entities.Get(inviterId.Value)!;
        var inviterParty = inviterE.Get<PartyState>()!;
        var inviterAppearance = inviterE.Get<PlayerAppearance>()!;

        if (inviterParty.Members.Count == Config.MaxPartyMembers - 1)
        {
            world.CurrentTick?.Events.Emit(new ChatMessageEvent { RecipientId = entityId.Value, Text = "The party is full.", ColorArgb = Color.White.ToArgb() });
            return;
        }

        for (byte i = 0; i < inviterParty.Members.Count; i++)
        {
            var memberId = inviterParty.Members[i];
            var memberE = world.Entities.Get(memberId)!;
            var memberParty = memberE.Get<PartyState>()!;
            memberParty.Members.Add(entityId);
            world.Dirty.Mark<PartyState>(memberId);
            if (memberId != inviterId.Value)
                party.Members.Add(memberId);
        }

        party.Members.Insert(0, inviterId.Value);
        inviterParty.Members.Add(entityId);
        party.Request = string.Empty;
        world.CurrentTick?.Events.Emit(new ChatMessageEvent { RecipientId = inviterId.Value.Value, Text = appearance.Name + " joined the party.", ColorArgb = Color.White.ToArgb() });

        world.Dirty.Mark<PartyState>(entityId);
        world.Dirty.Mark<PartyState>(inviterId.Value);
        for (byte i = 0; i < party.Members.Count; i++) world.Dirty.Mark<PartyState>(party.Members[i]);
    }

    internal void Decline(EntityId entityId)
    {
        var world = GameWorld.Current;
        var e = world.Entities.Get(entityId)!;
        var appearance = e.Get<PlayerAppearance>()!;
        var party = e.Get<PartyState>()!;

        var inviterId = world.FindPlayer(party.Request);
        if (inviterId != null) world.CurrentTick?.Events.Emit(new ChatMessageEvent { RecipientId = inviterId.Value.Value, Text = appearance.Name + " decline the party.", ColorArgb = Color.White.ToArgb() });
        party.Request = string.Empty;
    }

    public void Leave(EntityId entityId)
    {
        var world = GameWorld.Current;
        var e = world.Entities.Get(entityId)!;
        var party = e.Get<PartyState>()!;

        if (party.Members.Count == 0) return;

        for (byte i = 0; i < party.Members.Count; i++)
        {
            var memberE = world.Entities.Get(party.Members[i])!;
            var memberParty = memberE.Get<PartyState>()!;
            memberParty.Members.Remove(entityId);
        }

        for (byte i = 0; i < party.Members.Count; i++)
            world.Dirty.Mark<PartyState>(party.Members[i]);

        party.Members.Clear();
        world.Dirty.Mark<PartyState>(entityId);
    }

    public void Execute(GameWorld world, Tick tick)
    {
        foreach (var ev in tick.Events.Events)
        {
            if (ev is not PlayerDisconnectedEvent e) continue;
            var playerId = world.FindPlayerByValue(e.PlayerId);
            if (playerId != null) Leave(playerId.Value);
        }
    }
}
