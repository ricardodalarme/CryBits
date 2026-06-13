using CryBits.Definitions.Common;
using CryBits.Network;
using CryBits.Network.Packets.Client;
using CryBits.Host.Core;
using CryBits.Simulation.Intents;
using CryBits.Simulation.State;

namespace CryBits.Host.Services;

internal sealed class TradeService()
{
    public static TradeService Instance { get; } = new();

    [PacketHandler]
    internal void TradeInvite(EntityId entityId, TradeInvitePacket packet)
    {
        WorldHost.Current.CurrentTick?.Intents.Enqueue(new TradeInviteIntent(entityId, packet.PlayerName));
    }

    [PacketHandler]
    internal void TradeAccept(EntityId entityId, TradeAcceptPacket _)
    {
        WorldHost.Current.CurrentTick?.Intents.Enqueue(new TradeAcceptIntent(entityId));
    }

    [PacketHandler]
    internal void TradeDecline(EntityId entityId, TradeDeclinePacket _)
    {
        WorldHost.Current.CurrentTick?.Intents.Enqueue(new TradeDeclineIntent(entityId));
    }

    [PacketHandler]
    internal void TradeLeave(EntityId entityId, TradeLeavePacket _)
    {
        WorldHost.Current.CurrentTick?.Intents.Enqueue(new TradeLeaveIntent(entityId));
    }

    [PacketHandler]
    internal void TradeOffer(EntityId entityId, TradeOfferPacket packet)
    {
        WorldHost.Current.CurrentTick?.Intents.Enqueue(
            new TradeOfferIntent(entityId, packet.Slot, packet.InventorySlot, packet.Amount));
    }

    [PacketHandler]
    internal void TradeOfferState(EntityId entityId, TradeOfferStatePacket packet)
    {
        WorldHost.Current.CurrentTick?.Intents.Enqueue(
            new TradeOfferStateIntent(entityId, (TradeStatus)packet.State));
    }
}
