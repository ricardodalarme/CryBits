using CryBits.Protocol;
using CryBits.Protocol.Packets.Client;
using CryBits.Host.Core;
using CryBits.Simulation.Intents;
using CryBits.Simulation.State;

namespace CryBits.Host.Services;

internal sealed class PartyService(WorldHost host)
{
    [PacketHandler]
    internal void PartyInvite(EntityId entityId, PartyInvitePacket packet)
    {
        host.IntentFunnel.Submit(new PartyInviteIntent(entityId, packet.PlayerName));
    }

    [PacketHandler]
    internal void PartyAccept(EntityId entityId, PartyAcceptPacket _)
    {
        host.IntentFunnel.Submit(new PartyAcceptIntent(entityId));
    }

    [PacketHandler]
    internal void PartyDecline(EntityId entityId, PartyDeclinePacket _)
    {
        host.IntentFunnel.Submit(new PartyDeclineIntent(entityId));
    }

    [PacketHandler]
    internal void PartyLeave(EntityId entityId, PartyLeavePacket _)
    {
        host.IntentFunnel.Submit(new PartyLeaveIntent(entityId));
    }
}
