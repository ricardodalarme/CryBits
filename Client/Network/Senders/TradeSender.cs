using CryBits.Enums;
using CryBits.Packets.Client;

namespace CryBits.Client.Network.Senders;

internal static class TradeSender
{
    public static void TradeInvite(string playerName) =>
        Send.Packet(ClientPacket.TradeInvite, new TradeInvitePacket { PlayerName = playerName });

    public static void TradeAccept() => Send.Packet(ClientPacket.TradeAccept, new TradeAcceptPacket());

    public static void TradeDecline() => Send.Packet(ClientPacket.TradeDecline, new TradeDeclinePacket());

    public static void TradeLeave() => Send.Packet(ClientPacket.TradeLeave, new TradeLeavePacket());

    public static void TradeOffer(short slot, short inventorySlot, short amount = 1) =>
        Send.Packet(ClientPacket.TradeOffer,
            new TradeOfferPacket { Slot = slot, InventorySlot = inventorySlot, Amount = amount });

    public static void TradeOfferState(TradeStatus state) => Send.Packet(ClientPacket.TradeOfferState,
        new TradeOfferStatePacket { State = (byte)state });
}
