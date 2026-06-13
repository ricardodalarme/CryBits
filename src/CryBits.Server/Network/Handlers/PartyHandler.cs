using CryBits.Network;
using CryBits.Network.Packets.Client;
using CryBits.Server.Simulation.State;
using CryBits.Server.Systems.Party;

namespace CryBits.Server.Network.Handlers;

internal sealed class PartyHandler(PartySystem partySystem)
{
    public static PartyHandler Instance { get; } = new(PartySystem.Instance);

    [PacketHandler]
    internal void PartyInvite(EntityId entityId, PartyInvitePacket packet)
    {
        partySystem.Invite(entityId, packet.PlayerName);
    }

    [PacketHandler]
    internal void PartyAccept(EntityId entityId, PartyAcceptPacket _)
    {
        partySystem.Accept(entityId);
    }

    [PacketHandler]
    internal void PartyDecline(EntityId entityId, PartyDeclinePacket _)
    {
        partySystem.Decline(entityId);
    }

    [PacketHandler]
    internal void PartyLeave(EntityId entityId, PartyLeavePacket _)
    {
        partySystem.Leave(entityId);
    }
}
