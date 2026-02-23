using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Packets.Server;
using CryBits.Server.ECS;
using CryBits.Server.ECS.Components;
using CryBits.Server.Entities;
using CryBits.Server.World;
using static CryBits.Globals;

namespace CryBits.Server.Network.Senders;

internal static class TradeSender
{
    private static CryBits.Server.ECS.World World => ServerContext.Instance.World;

    public static void Trade(Player player, bool state)
    {
        Send.ToPlayer(player, new TradePacket { State = state });
    }

    public static void TradeInvitation(Player player, string playerInvitation)
    {
        Send.ToPlayer(player, new TradeInvitationPacket { PlayerInvitation = playerInvitation });
    }

    public static void TradeState(Player player, TradeStatus state)
    {
        Send.ToPlayer(player, new TradeStatePacket { State = (byte)state });
    }

    public static void TradeOffer(Player player, bool own = true)
    {
        var trade = World.Get<TradeComponent>(player.EntityId);
        // For own=false we want the partner's offer displayed to the player
        Player? source;
        if (own)
        {
            source = player;
        }
        else
        {
            var partner = trade.PartnerId.HasValue
                ? GameWorld.Current.Sessions.Find(s => s.IsPlaying && s.Character?.EntityId == trade.PartnerId.Value)?.Character
                : null;
            source = partner;
        }

        if (source == null) return;

        var sourceTrade = World.Get<TradeComponent>(source.EntityId);
        var sourceInv   = World.Get<InventoryComponent>(source.EntityId);

        var packet = new TradeOfferPacket
        {
            Own   = own,
            Items = new PacketsTradeOfferItem[MaxInventory]
        };
        for (short i = 0; i < MaxInventory; i++)
        {
            var slotNum = sourceTrade.Offer![i].SlotNum;
            packet.Items[i] = new PacketsTradeOfferItem
            {
                ItemId = sourceInv.Slots[slotNum].Item.GetId(),
                Amount = sourceTrade.Offer[i].Amount
            };
        }
        Send.ToPlayer(player, packet);
    }
}
