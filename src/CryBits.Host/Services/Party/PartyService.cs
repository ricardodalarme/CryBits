using CryBits.Host.Core;
using CryBits.Host.Network;
using CryBits.Host.Network.Senders;
using CryBits.Protocol.Packets.Server;
using CryBits.Simulation.Core;
using CryBits.Simulation.Intents;
using CryBits.Simulation.State;
using CryBits.Host.Ingress;
using System.Drawing;
using static CryBits.Definitions.Globals;

namespace CryBits.Host.Services.Party;

public sealed class PartyService(
    IntentFunnel funnel,
    PackageSender sender,
    ChatSender chatSender,
    SessionManager sessions,
    World world)
{
    private readonly List<PartySession> _activeParties = [];
    private readonly Dictionary<EntityId, EntityId> _pendingInvitations = [];

    public void HandleInvite(EntityId inviterId, string inviteeName)
    {
        var inviteeId = world.FindPlayer(inviteeName);
        if (inviteeId == null || inviteeId == inviterId)
        {
            chatSender.Message(inviterId, "The player isn't connected.", Color.White);
            return;
        }

        var inviteeSession = sessions.Get(inviteeId.Value);
        var inviterSession = sessions.Get(inviterId);
        if (inviteeSession == null || inviterSession == null) return;

        var inviteeParty = GetParty(inviteeId.Value);
        if (inviteeParty != null)
        {
            chatSender.Message(inviterId, "The player is already part of a party.", Color.White);
            return;
        }

        if (_pendingInvitations.ContainsKey(inviteeId.Value))
        {
            chatSender.Message(inviterId, "The player is analyzing an invitation to another party.", Color.White);
            return;
        }

        var inviterParty = GetParty(inviterId);
        if (inviterParty != null && inviterParty.Members.Count >= Config.MaxPartyMembers)
        {
            chatSender.Message(inviterId, "Your party is full.", Color.White);
            return;
        }

        _pendingInvitations[inviteeId.Value] = inviterId;

        var inviterName = world.Entities.Get(inviterId)?.Get<Simulation.Components.PlayerAppearance>()?.Name ?? string.Empty;
        sender.ToPlayer(inviteeId.Value, new PartyInvitationPacket { PlayerInvitation = inviterName });
    }

    public void HandleAccept(EntityId inviteeId)
    {
        if (!_pendingInvitations.Remove(inviteeId, out var inviterId)) return;

        var inviteeSession = sessions.Get(inviteeId);
        var inviterSession = sessions.Get(inviterId);
        if (inviteeSession == null || inviterSession == null) return;

        var inviterParty = GetParty(inviterId);
        if (inviterParty == null)
        {
            inviterParty = new PartySession(inviterId);
            _activeParties.Add(inviterParty);
        }

        if (inviterParty.Members.Count >= Config.MaxPartyMembers)
        {
            chatSender.Message(inviteeId, "The party is full.", Color.White);
            return;
        }

        inviterParty.Members.Add(inviteeId);
        SyncPartyToSimulation(inviterParty);
        BroadcastPartyUpdate(inviterParty);

        var inviteeName = world.Entities.Get(inviteeId)?.Get<Simulation.Components.PlayerAppearance>()?.Name ?? string.Empty;
        var inviterName = world.Entities.Get(inviterId)?.Get<Simulation.Components.PlayerAppearance>()?.Name ?? string.Empty;

        chatSender.Message(inviteeId, "You joined " + inviterName + "'s party.", Color.White);
        chatSender.Message(inviterId, inviteeName + " joined the party.", Color.White);
    }

    public void HandleDecline(EntityId inviteeId)
    {
        if (_pendingInvitations.Remove(inviteeId, out var inviterId))
        {
            var inviteeName = world.Entities.Get(inviteeId)?.Get<Simulation.Components.PlayerAppearance>()?.Name ?? string.Empty;
            chatSender.Message(inviterId, inviteeName + " declined the party invitation.", Color.White);
        }
    }

    public void HandleLeave(EntityId entityId)
    {
        var party = GetParty(entityId);
        if (party == null) return;

        party.Members.Remove(entityId);

        // Remove XP share component for the leaving player
        funnel.Submit(new XpShareIntent(entityId, []));
        sender.ToPlayer(entityId, new PartyPacket { MemberIds = [] });

        if (party.Members.Count <= 1)
        {
            // Disband party
            if (party.Members.Count == 1)
            {
                var remaining = party.Members[0];
                funnel.Submit(new XpShareIntent(remaining, []));
                sender.ToPlayer(remaining, new PartyPacket { MemberIds = [] });
            }
            _activeParties.Remove(party);
        }
        else
        {
            if (party.Leader == entityId)
            {
                party.Leader = party.Members[0];
            }
            SyncPartyToSimulation(party);
            BroadcastPartyUpdate(party);
        }
    }

    public void HandleDisconnect(EntityId entityId)
    {
        _pendingInvitations.Remove(entityId);
        HandleLeave(entityId);
    }

    private PartySession? GetParty(EntityId entityId)
    {
        return _activeParties.Find(p => p.Members.Contains(entityId));
    }

    private void SyncPartyToSimulation(PartySession party)
    {
        foreach (var memberId in party.Members)
        {
            // Exclude self from recipients list for leveling XP calculations inside sim
            var recipients = party.Members.Where(id => id != memberId).ToList();
            funnel.Submit(new XpShareIntent(memberId, recipients));
        }
    }

    private void BroadcastPartyUpdate(PartySession party)
    {
        // Packets expect long array representing character server IDs
        var memberNetworkIds = party.Members.Select(m => sessions.Get(m)?.Character?.Value ?? 0L).ToArray();
        foreach (var memberId in party.Members)
        {
            sender.ToPlayer(memberId, new PartyPacket { MemberIds = memberNetworkIds });
        }
    }
}
