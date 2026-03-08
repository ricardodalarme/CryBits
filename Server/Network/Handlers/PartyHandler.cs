using CryBits.Packets.Client;
using CryBits.Server.Entities;
using CryBits.Server.Systems;

namespace CryBits.Server.Network.Handlers;

internal sealed class PartyHandler(PartySystem partySystem)
{
    public static PartyHandler Instance { get; } = new(PartySystem.Instance);

    private readonly PartySystem _partySystem = partySystem;

    [PacketHandler]
    internal void PartyInvite(Player player, PartyInvitePacket packet)
    {
        _partySystem.Invite(player, packet.PlayerName);
    }

    [PacketHandler]
    internal void PartyAccept(Player player, PartyAcceptPacket _)
    {
        _partySystem.Accept(player);
    }

    [PacketHandler]
    internal void PartyDecline(Player player, PartyDeclinePacket _)
    {
        _partySystem.Decline(player);
    }

    [PacketHandler]
    internal void PartyLeave(Player player, PartyLeavePacket _)
    {
        _partySystem.Leave(player);
    }
}
