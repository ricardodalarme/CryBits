using CryBits.Transport;
using CryBits.Transport.Packets.Client;
using CryBits.Host.Core;
using CryBits.Simulation.Intents;
using CryBits.Simulation.State;

namespace CryBits.Host.Services;

internal sealed class PartyService()
{
    public static PartyService Instance { get; } = new();

    [PacketHandler]
    internal void PartyInvite(EntityId entityId, PartyInvitePacket packet)
    {
        WorldHost.Current.CurrentTick?.Intents.Enqueue(new PartyInviteIntent(entityId, packet.PlayerName));
    }

    [PacketHandler]
    internal void PartyAccept(EntityId entityId, PartyAcceptPacket _)
    {
        WorldHost.Current.CurrentTick?.Intents.Enqueue(new PartyAcceptIntent(entityId));
    }

    [PacketHandler]
    internal void PartyDecline(EntityId entityId, PartyDeclinePacket _)
    {
        WorldHost.Current.CurrentTick?.Intents.Enqueue(new PartyDeclineIntent(entityId));
    }

    [PacketHandler]
    internal void PartyLeave(EntityId entityId, PartyLeavePacket _)
    {
        WorldHost.Current.CurrentTick?.Intents.Enqueue(new PartyLeaveIntent(entityId));
    }
}
