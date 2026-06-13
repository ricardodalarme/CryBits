using CryBits.Network;
using CryBits.Network.Packets.Client;
using CryBits.Server.Core;
using CryBits.Simulation.Intents;
using CryBits.Simulation.State;

namespace CryBits.Server.Network.Handlers;

internal sealed class PartyHandler()
{
    public static PartyHandler Instance { get; } = new();

    [PacketHandler]
    internal void PartyInvite(EntityId entityId, PartyInvitePacket packet)
    {
        GameWorld.Current.CurrentTick?.Intents.Enqueue(new PartyInviteIntent(entityId, packet.PlayerName));
    }

    [PacketHandler]
    internal void PartyAccept(EntityId entityId, PartyAcceptPacket _)
    {
        GameWorld.Current.CurrentTick?.Intents.Enqueue(new PartyAcceptIntent(entityId));
    }

    [PacketHandler]
    internal void PartyDecline(EntityId entityId, PartyDeclinePacket _)
    {
        GameWorld.Current.CurrentTick?.Intents.Enqueue(new PartyDeclineIntent(entityId));
    }

    [PacketHandler]
    internal void PartyLeave(EntityId entityId, PartyLeavePacket _)
    {
        GameWorld.Current.CurrentTick?.Intents.Enqueue(new PartyLeaveIntent(entityId));
    }
}
