using CryBits.Packets.Client;
using CryBits.Server.Entities;
using CryBits.Server.Systems;

namespace CryBits.Server.Network.Handlers;

internal static class PartyHandler
{
    [PacketHandler]
    internal static void PartyInvite(Player player, PartyInvitePacket packet)
    {
        PartySystem.Invite(player, packet.PlayerName);
    }

    [PacketHandler]
    internal static void PartyAccept(Player player, PartyAcceptPacket _)
    {
        PartySystem.Accept(player);
    }

    [PacketHandler]
    internal static void PartyDecline(Player player, PartyDeclinePacket _)
    {
        PartySystem.Decline(player);
    }

    [PacketHandler]
    internal static void PartyLeave(Player player, PartyLeavePacket _)
    {
        PartySystem.Leave(player);
    }
}
