using CryBits.Enums;
using CryBits.Packets.Client;
using CryBits.Server.Entities;
using CryBits.Server.Systems;

namespace CryBits.Server.Network.Handlers;

internal static class PartyHandler
{
    [PacketHandler(ClientPacket.PartyInvite)]
    internal static void PartyInvite(Player player, PartyInvitePacket packet)
    {
        PartySystem.Invite(player, packet.PlayerName);
    }

    [PacketHandler(ClientPacket.PartyAccept)]
    internal static void PartyAccept(Player player)
    {
        PartySystem.Accept(player);
    }

    [PacketHandler(ClientPacket.PartyDecline)]
    internal static void PartyDecline(Player player)
    {
        PartySystem.Decline(player);
    }

    [PacketHandler(ClientPacket.PartyLeave)]
    internal static void PartyLeave(Player player)
    {
        PartySystem.Leave(player);
    }
}
