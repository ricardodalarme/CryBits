using CryBits.Packets.Client;

namespace CryBits.Client.Network.Senders;

internal static class PartySender
{
    public static void PartyInvite(string playerName) =>
        PacketSender.Packet(new PartyInvitePacket { PlayerName = playerName });

    public static void PartyAccept() => PacketSender.Packet(new PartyAcceptPacket());

    public static void PartyDecline() => PacketSender.Packet(new PartyDeclinePacket());

    public static void PartyLeave() => PacketSender.Packet(new PartyLeavePacket());
}
