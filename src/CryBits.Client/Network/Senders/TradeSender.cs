using CryBits.Client.Framework.Network;
using CryBits.Definitions.Common;
using CryBits.Protocol.Packets.Client;
using CryBits.Transport;

namespace CryBits.Client.Network.Senders;

internal class TradeSender(PacketSender packetSender)
{
    public static TradeSender Instance { get; } = new(PacketSender.Instance);

    public void TradeInvite(string playerName) =>
        packetSender.Packet(new TradeInvitePacket { PlayerName = playerName }, DeliveryChannel.ReliableUnordered);

    public void TradeAccept() => packetSender.Packet(new TradeAcceptPacket(), DeliveryChannel.ReliableUnordered);

    public void TradeDecline() => packetSender.Packet(new TradeDeclinePacket(), DeliveryChannel.ReliableUnordered);

    public void TradeLeave() => packetSender.Packet(new TradeLeavePacket(), DeliveryChannel.ReliableUnordered);

    public void TradeOffer(short slot, short inventorySlot, short amount = 1) =>
        packetSender.Packet(
            new TradeOfferPacket { Slot = slot, InventorySlot = inventorySlot, Amount = amount });

    public void TradeOfferState(TradeStatus state) =>
        packetSender.Packet(new TradeOfferStatePacket { State = (byte)state });
}
