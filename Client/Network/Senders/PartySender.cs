using CryBits.Client.Framework.Network;
using CryBits.Packets.Client;

namespace CryBits.Client.Network.Senders;

internal class PartySender(PacketSender packetSender)
{
    public static PartySender Instance { get; } = new(PacketSender.Instance);

    public void PartyInvite(string playerName) =>
        packetSender.Packet(new PartyInvitePacket { PlayerName = playerName });

    public void PartyAccept() => packetSender.Packet(new PartyAcceptPacket());

    public void PartyDecline() => packetSender.Packet(new PartyDeclinePacket());

    public void PartyLeave() => packetSender.Packet(new PartyLeavePacket());
}
