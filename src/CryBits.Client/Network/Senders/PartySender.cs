using CryBits.Client.Framework.Network;
using CryBits.Packets.Client;
using LiteNetLib;

namespace CryBits.Client.Network.Senders;

internal class PartySender(PacketSender packetSender)
{
    public static PartySender Instance { get; } = new(PacketSender.Instance);

    public void PartyInvite(string playerName) =>
        packetSender.Packet(new PartyInvitePacket { PlayerName = playerName }, DeliveryMethod.ReliableUnordered);

    public void PartyAccept() => packetSender.Packet(new PartyAcceptPacket(), DeliveryMethod.ReliableUnordered);

    public void PartyDecline() => packetSender.Packet(new PartyDeclinePacket(), DeliveryMethod.ReliableUnordered);

    public void PartyLeave() => packetSender.Packet(new PartyLeavePacket(), DeliveryMethod.ReliableUnordered);
}
