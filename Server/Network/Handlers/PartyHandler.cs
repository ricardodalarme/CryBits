using CryBits.Packets.Client;
using CryBits.Server.Entities;
using CryBits.Server.Systems;

namespace CryBits.Server.Network.Handlers;

internal static class PartyHandler
{
    internal static void PartyInvite(Player player, PartyInvitePacket packet)
    {
        PartySystem.Invite(player, packet.PlayerName);
    }

    internal static void PartyAccept(Player player)
    {
        PartySystem.Accept(player);
    }

    internal static void PartyDecline(Player player)
    {
        PartySystem.Decline(player);
    }

    internal static void PartyLeave(Player player)
    {
        PartySystem.Leave(player);
    }
}
