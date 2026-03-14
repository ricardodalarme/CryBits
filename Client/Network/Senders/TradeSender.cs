using CryBits.Client.Framework.Network;
using CryBits.Enums;
using CryBits.Packets.Client;

namespace CryBits.Client.Network.Senders;

internal class TradeSender(PacketSender packetSender)
{
    public static TradeSender Instance { get; } = new(PacketSender.Instance);

    public void TradeInvite(string playerName) =>
        packetSender.Packet(new TradeInvitePacket { PlayerName = playerName });

    public void TradeAccept() => packetSender.Packet(new TradeAcceptPacket());

    public void TradeDecline() => packetSender.Packet(new TradeDeclinePacket());

    public void TradeLeave() => packetSender.Packet(new TradeLeavePacket());

    public void TradeOffer(short slot, short inventorySlot, short amount = 1) =>
        packetSender.Packet(
            new TradeOfferPacket { Slot = slot, InventorySlot = inventorySlot, Amount = amount });

    public void TradeOfferState(TradeStatus state) => packetSender.Packet(
        new TradeOfferStatePacket { State = (byte)state });
}
