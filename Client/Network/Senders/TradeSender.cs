using CryBits.Enums;
using CryBits.Packets.Client;

namespace CryBits.Client.Network.Senders;

internal static class TradeSender
{
    public static void TradeInvite(string playerName) =>
        PacketSender.Packet(new TradeInvitePacket { PlayerName = playerName });

    public static void TradeAccept() => PacketSender.Packet(new TradeAcceptPacket());

    public static void TradeDecline() => PacketSender.Packet(new TradeDeclinePacket());

    public static void TradeLeave() => PacketSender.Packet(new TradeLeavePacket());

    public static void TradeOffer(short slot, short inventorySlot, short amount = 1) =>
        PacketSender.Packet(
            new TradeOfferPacket { Slot = slot, InventorySlot = inventorySlot, Amount = amount });

    public static void TradeOfferState(TradeStatus state) => PacketSender.Packet(
        new TradeOfferStatePacket { State = (byte)state });
}
