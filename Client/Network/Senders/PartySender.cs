using CryBits.Enums;
using CryBits.Packets.Client;

namespace CryBits.Client.Network.Senders;

internal static class PartySender
{
    public static void PartyInvite(string playerName) =>
        Send.Packet(ClientPacket.PartyInvite, new PartyInvitePacket { PlayerName = playerName });

    public static void PartyAccept() => Send.Packet(ClientPacket.PartyAccept, new PartyAcceptPacket());

    public static void PartyDecline() => Send.Packet(ClientPacket.PartyDecline, new PartyDeclinePacket());

    public static void PartyLeave() => Send.Packet(ClientPacket.PartyLeave, new PartyLeavePacket());
}
