using CryBits.Enums;
using CryBits.Packets.Client;

namespace CryBits.Client.Network.Senders;

internal static class TradeSender
{
    public static void TradeInvite(string playerName) =>
        Send.Packet(new TradeInvitePacket { PlayerName = playerName });

    public static void TradeAccept() => Send.Packet(new TradeAcceptPacket());

    public static void TradeDecline() => Send.Packet(new TradeDeclinePacket());

    public static void TradeLeave() => Send.Packet(new TradeLeavePacket());

    public static void TradeOffer(short slot, short inventorySlot, short amount = 1) =>
        Send.Packet(
            new TradeOfferPacket { Slot = slot, InventorySlot = inventorySlot, Amount = amount });

    public static void TradeOfferState(TradeStatus state) => Send.Packet(
        new TradeOfferStatePacket { State = (byte)state });
}
