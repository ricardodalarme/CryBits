using CryBits.Definitions.Common;
using CryBits.Transport;
using CryBits.Transport.Packets.Client;
using CryBits.Host.Core;
using CryBits.Simulation.Intents;
using CryBits.Simulation.State;

namespace CryBits.Host.Services;

internal sealed class TradeService(WorldHost host)
{
    [PacketHandler]
    internal void TradeInvite(EntityId entityId, TradeInvitePacket packet)
    {
        host.CurrentTick?.Intents.Enqueue(new TradeInviteIntent(entityId, packet.PlayerName));
    }

    [PacketHandler]
    internal void TradeAccept(EntityId entityId, TradeAcceptPacket _)
    {
        host.CurrentTick?.Intents.Enqueue(new TradeAcceptIntent(entityId));
    }

    [PacketHandler]
    internal void TradeDecline(EntityId entityId, TradeDeclinePacket _)
    {
        host.CurrentTick?.Intents.Enqueue(new TradeDeclineIntent(entityId));
    }

    [PacketHandler]
    internal void TradeLeave(EntityId entityId, TradeLeavePacket _)
    {
        host.CurrentTick?.Intents.Enqueue(new TradeLeaveIntent(entityId));
    }

    [PacketHandler]
    internal void TradeOffer(EntityId entityId, TradeOfferPacket packet)
    {
        host.CurrentTick?.Intents.Enqueue(
            new TradeOfferIntent(entityId, packet.Slot, packet.InventorySlot, packet.Amount));
    }

    [PacketHandler]
    internal void TradeOfferState(EntityId entityId, TradeOfferStatePacket packet)
    {
        host.CurrentTick?.Intents.Enqueue(
            new TradeOfferStateIntent(entityId, (TradeStatus)packet.State));
    }
}
