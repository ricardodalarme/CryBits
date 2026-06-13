using CryBits.Definitions.Helpers.Extensions;
using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;
using CryBits.Server.Simulation.Core;
using CryBits.Server.Simulation.State;
using CryBits.Server.Simulation.State.Components;
using CryBits.Simulation.Events;
using CryBits.Server.World;
using System.Drawing;
using static CryBits.Definitions.Globals;
using CryBits.Simulation.Core;

namespace CryBits.Server.Systems.Party;

internal sealed class PartySystem(ChatSender chatSender, PartySender partySender) : ISimulationSystem
{
    public static PartySystem Instance { get; } = new(ChatSender.Instance, PartySender.Instance);

    internal void Invite(EntityId entityId, string targetName)
    {
        var world = GameWorld.Current;
        var e = world.Entities.Get(entityId)!;
        var appearance = e.Get<PlayerAppearance>()!;
        var party = e.Get<PartyState>()!;

        var invitedId = world.FindPlayer(targetName);

        if (invitedId == null)
        {
            chatSender.Message(entityId, "The player isn't connected.", Color.White);
            return;
        }

        if (invitedId.Value == entityId)
        {
            chatSender.Message(entityId, "You can't be invited.", Color.White);
            return;
        }

        var invitedE = world.Entities.Get(invitedId.Value)!;
        var invitedParty = invitedE.Get<PartyState>()!;

        if (invitedParty.Members.Count != 0)
        {
            chatSender.Message(entityId, "The player is already part of a party.", Color.White);
            return;
        }

        if (!string.IsNullOrEmpty(invitedParty.Request))
        {
            chatSender.Message(entityId, "The player is analyzing an invitation to another party.", Color.White);
            return;
        }

        if (party.Members.Count == Config.MaxPartyMembers - 1)
        {
            chatSender.Message(entityId, "Your party is full.", Color.White);
            return;
        }

        invitedParty.Request = appearance.Name;
        partySender.PartyInvitation(invitedId.Value, appearance.Name);
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
            chatSender.Message(entityId, "You are already part of a party.", Color.White);
            return;
        }

        if (inviterId == null)
        {
            chatSender.Message(entityId, "Who invited you is no longer available.", Color.White);
            return;
        }

        var inviterE = world.Entities.Get(inviterId.Value)!;
        var inviterParty = inviterE.Get<PartyState>()!;
        var inviterAppearance = inviterE.Get<PlayerAppearance>()!;

        if (inviterParty.Members.Count == Config.MaxPartyMembers - 1)
        {
            chatSender.Message(entityId, "The party is full.", Color.White);
            return;
        }

        for (byte i = 0; i < inviterParty.Members.Count; i++)
        {
            var memberId = inviterParty.Members[i];
            var memberE = world.Entities.Get(memberId)!;
            var memberParty = memberE.Get<PartyState>()!;
            memberParty.Members.Add(entityId);
            if (memberId != inviterId.Value)
                party.Members.Add(memberId);
        }

        party.Members.Insert(0, inviterId.Value);
        inviterParty.Members.Add(entityId);
        party.Request = string.Empty;
        chatSender.Message(inviterId.Value, appearance.Name + " joined the party.", Color.White);

        partySender.Party(entityId);
        for (byte i = 0; i < party.Members.Count; i++) partySender.Party(party.Members[i]);
    }

    internal void Decline(EntityId entityId)
    {
        var world = GameWorld.Current;
        var e = world.Entities.Get(entityId)!;
        var appearance = e.Get<PlayerAppearance>()!;
        var party = e.Get<PartyState>()!;

        var inviterId = world.FindPlayer(party.Request);
        if (inviterId != null) chatSender.Message(inviterId.Value, appearance.Name + " decline the party.", Color.White);
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
            partySender.Party(party.Members[i]);

        party.Members.Clear();
        partySender.Party(entityId);
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
