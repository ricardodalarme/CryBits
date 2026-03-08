using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Packets.Server;
using CryBits.Server.Entities;
using CryBits.Server.Network;
using static CryBits.Globals;

namespace CryBits.Server.Network.Senders;

internal sealed class TradeSender(PackageSender packageSender)
{
    public static TradeSender Instance { get; } = new(PackageSender.Instance);

    private readonly PackageSender _packageSender = packageSender;

    public void Trade(Player player, bool state)
    {
        _packageSender.ToPlayer(player, new TradePacket { State = state });
    }

    public void TradeInvitation(Player player, string playerInvitation)
    {
        _packageSender.ToPlayer(player, new TradeInvitationPacket { PlayerInvitation = playerInvitation });
    }

    public void TradeState(Player player, TradeStatus state)
    {
        _packageSender.ToPlayer(player, new TradeStatePacket { State = (byte)state });
    }

    public void TradeOffer(Player player, bool own = true)
    {
        var to = own ? player : player.Trade;
        var packet = new TradeOfferPacket
        {
            Own = own,
            Items = new PacketsTradeOfferItem[MaxInventory]
        };
        for (short i = 0; i < MaxInventory; i++)
        {
            packet.Items[i] = new PacketsTradeOfferItem
            {
                ItemId = to.Inventory[to.TradeOffer[i].SlotNum].Item.GetId(),
                Amount = to.TradeOffer[i].Amount
            };
        }
        _packageSender.ToPlayer(player, packet);
    }
}
