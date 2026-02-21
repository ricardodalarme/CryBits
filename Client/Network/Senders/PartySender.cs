using CryBits.Packets.Client;

namespace CryBits.Client.Network.Senders;

internal static class PartySender
{
    public static void PartyInvite(string playerName) =>
        Send.Packet(new PartyInvitePacket { PlayerName = playerName });

    public static void PartyAccept() => Send.Packet(new PartyAcceptPacket());

    public static void PartyDecline() => Send.Packet(new PartyDeclinePacket());

    public static void PartyLeave() => Send.Packet(new PartyLeavePacket());
}
