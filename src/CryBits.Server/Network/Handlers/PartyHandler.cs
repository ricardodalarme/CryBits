using CryBits.Packets.Client;
using CryBits.Server.Entities;
using CryBits.Server.Systems;

namespace CryBits.Server.Network.Handlers;

internal sealed class PartyHandler(PartySystem partySystem)
{
    public static PartyHandler Instance { get; } = new(PartySystem.Instance);

    [PacketHandler]
    internal void PartyInvite(Player player, PartyInvitePacket packet)
    {
        partySystem.Invite(player, packet.PlayerName);
    }

    [PacketHandler]
    internal void PartyAccept(Player player, PartyAcceptPacket _)
    {
        partySystem.Accept(player);
    }

    [PacketHandler]
    internal void PartyDecline(Player player, PartyDeclinePacket _)
    {
        partySystem.Decline(player);
    }

    [PacketHandler]
    internal void PartyLeave(Player player, PartyLeavePacket _)
    {
        partySystem.Leave(player);
    }
}
