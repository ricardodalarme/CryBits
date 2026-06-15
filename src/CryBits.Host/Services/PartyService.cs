using CryBits.Transport;
using CryBits.Transport.Packets.Client;
using CryBits.Host.Core;
using CryBits.Simulation.Intents;
using CryBits.Simulation.State;

namespace CryBits.Host.Services;

internal sealed class PartyService(WorldHost host)
{
    [PacketHandler]
    internal void PartyInvite(EntityId entityId, PartyInvitePacket packet)
    {
        host.CurrentTick?.Intents.Enqueue(new PartyInviteIntent(entityId, packet.PlayerName));
    }

    [PacketHandler]
    internal void PartyAccept(EntityId entityId, PartyAcceptPacket _)
    {
        host.CurrentTick?.Intents.Enqueue(new PartyAcceptIntent(entityId));
    }

    [PacketHandler]
    internal void PartyDecline(EntityId entityId, PartyDeclinePacket _)
    {
        host.CurrentTick?.Intents.Enqueue(new PartyDeclineIntent(entityId));
    }

    [PacketHandler]
    internal void PartyLeave(EntityId entityId, PartyLeavePacket _)
    {
        host.CurrentTick?.Intents.Enqueue(new PartyLeaveIntent(entityId));
    }
}
