using CryBits.Client.Framework.Network;
using CryBits.Protocol.Packets.Client;
using CryBits.Transport;

namespace CryBits.Client.Network.Senders;

internal class PartySender(PacketSender packetSender)
{
    public static PartySender Instance { get; } = new(PacketSender.Instance);

    public void PartyInvite(string playerName) =>
        packetSender.Packet(new PartyInvitePacket { PlayerName = playerName }, DeliveryChannel.ReliableUnordered);

    public void PartyAccept() => packetSender.Packet(new PartyAcceptPacket(), DeliveryChannel.ReliableUnordered);

    public void PartyDecline() => packetSender.Packet(new PartyDeclinePacket(), DeliveryChannel.ReliableUnordered);

    public void PartyLeave() => packetSender.Packet(new PartyLeavePacket(), DeliveryChannel.ReliableUnordered);
}
