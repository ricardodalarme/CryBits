using CryBits.Definitions.Common;
using CryBits.Network;
using CryBits.Network.Packets.Client;
using CryBits.Server.Simulation.State;
using CryBits.Server.Systems.Trade;

namespace CryBits.Server.Network.Handlers;

internal sealed class TradeHandler(TradeSystem tradeSystem)
{
    public static TradeHandler Instance { get; } = new(TradeSystem.Instance);

    [PacketHandler]
    internal void TradeInvite(EntityId entityId, TradeInvitePacket packet)
    {
        tradeSystem.Invite(entityId, packet.PlayerName);
    }

    [PacketHandler]
    internal void TradeAccept(EntityId entityId, TradeAcceptPacket _)
    {
        tradeSystem.Accept(entityId);
    }

    [PacketHandler]
    internal void TradeDecline(EntityId entityId, TradeDeclinePacket _)
    {
        tradeSystem.Decline(entityId);
    }

    [PacketHandler]
    internal void TradeLeave(EntityId entityId, TradeLeavePacket _)
    {
        tradeSystem.Leave(entityId);
    }

    [PacketHandler]
    internal void TradeOffer(EntityId entityId, TradeOfferPacket packet)
    {
        tradeSystem.Offer(entityId, packet.Slot, packet.InventorySlot, packet.Amount);
    }

    [PacketHandler]
    internal void TradeOfferState(EntityId entityId, TradeOfferStatePacket packet)
    {
        tradeSystem.OfferState(entityId, (TradeStatus)packet.State);
    }
}
