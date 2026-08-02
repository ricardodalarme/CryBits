using CryBits.Definitions.Common;
using CryBits.Definitions.Slots;
using CryBits.Host.Core;
using CryBits.Host.Ingress;
using CryBits.Host.Network;
using CryBits.Protocol.Packets.Server;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Intents;
using static CryBits.Definitions.Globals;

namespace CryBits.Host.Services.Trade;

public sealed class TradeService(
    IntentFunnel funnel,
    PackageSender sender,
    SessionManager sessions,
    World world)
{
    private readonly Dictionary<EntityId, TradeSession> _activeTrades = [];
    private readonly Dictionary<EntityId, EntityId> _pendingInvitations = [];

    public void HandleInvite(EntityId inviterId, string inviteeName)
    {
        var inviteeId = world.FindPlayer(inviteeName);
        if (inviteeId == null || inviteeId == inviterId) return;

        var inviteeSession = sessions.Get(inviteeId.Value);
        var inviterSession = sessions.Get(inviterId);
        if (inviteeSession == null || inviterSession == null) return;

        if (_activeTrades.ContainsKey(inviteeId.Value) || _activeTrades.ContainsKey(inviterId)) return;

        _pendingInvitations[inviteeId.Value] = inviterId;

        var inviterName = world.Entities.Get<PlayerAppearance>(inviterId)?.Name ?? string.Empty;
        sender.ToPlayer(inviteeId.Value, new TradeInvitationPacket { PlayerInvitation = inviterName });
    }

    public void HandleAccept(EntityId inviteeId)
    {
        if (!_pendingInvitations.Remove(inviteeId, out var inviterId)) return;

        var session = new TradeSession(inviterId, inviteeId);
        _activeTrades[inviterId] = session;
        _activeTrades[inviteeId] = session;

        sender.ToPlayer(inviterId, new TradePacket { State = true });
        sender.ToPlayer(inviteeId, new TradePacket { State = true });
    }

    public void HandleDecline(EntityId inviteeId)
    {
        if (_pendingInvitations.Remove(inviteeId, out var inviterId)) sender.ToPlayer(inviterId, new TradeStatePacket { State = (byte)TradeStatus.Declined });
    }

    public void HandleLeave(EntityId entityId)
    {
        if (_activeTrades.Remove(entityId, out var session))
        {
            var partner = session.EntityA == entityId ? session.EntityB : session.EntityA;
            _activeTrades.Remove(partner);

            sender.ToPlayer(entityId, new TradePacket { State = false });
            sender.ToPlayer(partner, new TradePacket { State = false });
        }
    }

    public void HandleOffer(EntityId entityId, short slot, short inventorySlot, short amount)
    {
        if (!_activeTrades.TryGetValue(entityId, out var session)) return;

        var inv = world.Entities.Get<InventoryState>(entityId);
        if (inv == null) return;

        amount = Math.Min(amount, inv.Slots[inventorySlot].Amount);

        var isA = session.EntityA == entityId;
        var offer = isA ? session.OfferA : session.OfferB;
        var partner = isA ? session.EntityB : session.EntityA;

        var newOffer = (TradeSlot[])offer.Clone();
        if (inventorySlot != 0)
        {
            for (byte i = 0; i < MaxInventory; i++)
                if (newOffer[i].SlotNum == inventorySlot)
                    return;

            newOffer[slot] = new TradeSlot { SlotNum = inventorySlot, Amount = amount };
        }
        else
        {
            newOffer[slot] = new TradeSlot();
        }

        if (isA) session.OfferA = newOffer;
        else session.OfferB = newOffer;

        session.StatusA = TradeStatus.Waiting;
        session.StatusB = TradeStatus.Waiting;

        // Send updates
        var ownOfferItems = Array.ConvertAll(newOffer,
            s => new PacketsTradeOfferItem { ItemId = inv.Slots[s.SlotNum].ItemId, Amount = s.Amount });
        sender.ToPlayer(entityId, new TradeOfferPacket { Own = true, Items = ownOfferItems });
        sender.ToPlayer(partner, new TradeOfferPacket { Own = false, Items = ownOfferItems });
    }

    public void HandleOfferState(EntityId entityId, TradeStatus state)
    {
        if (!_activeTrades.TryGetValue(entityId, out var session)) return;

        var isA = session.EntityA == entityId;
        if (isA) session.StatusA = state;
        else session.StatusB = state;

        var partner = isA ? session.EntityB : session.EntityA;

        sender.ToPlayer(partner, new TradeStatePacket { State = (byte)state });

        if (session.StatusA == TradeStatus.Confirmed && session.StatusB == TradeStatus.Confirmed) CommitTrade(session);
    }

    private void CommitTrade(TradeSession session)
    {
        var invA = world.Entities.Get<InventoryState>(session.EntityA);
        var invB = world.Entities.Get<InventoryState>(session.EntityB);
        if (invA == null || invB == null) return;

        // Construct commit intent items list
        var itemsA = session.OfferA
            .Where(s => s.SlotNum > 0)
            .Select(s => new TradeCommitItem(invA.Slots[s.SlotNum].ItemId, s.SlotNum, s.Amount))
            .ToArray();

        var itemsB = session.OfferB
            .Where(s => s.SlotNum > 0)
            .Select(s => new TradeCommitItem(invB.Slots[s.SlotNum].ItemId, s.SlotNum, s.Amount))
            .ToArray();

        funnel.Submit(new TradeCommitIntent(session.EntityA, session.EntityB, itemsA, itemsB));

        // Clean session
        _activeTrades.Remove(session.EntityA);
        _activeTrades.Remove(session.EntityB);

        sender.ToPlayer(session.EntityA, new TradePacket { State = false });
        sender.ToPlayer(session.EntityB, new TradePacket { State = false });
    }
}
