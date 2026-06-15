using CryBits.Definitions.Common;
using CryBits.Transport.Packets.Server;
using CryBits.Simulation.Components;
using static CryBits.Definitions.Globals;
using CryBits.Simulation.State;

namespace CryBits.Host.Network.Senders;

internal sealed class TradeSender(PackageSender packageSender, EntityRegistry entities)
{
    public void Trade(EntityId entityId, bool state)
    {
        packageSender.ToPlayer(entityId, new TradePacket { State = state });
    }

    public void TradeInvitation(EntityId entityId, string playerInvitation)
    {
        packageSender.ToPlayer(entityId, new TradeInvitationPacket { PlayerInvitation = playerInvitation });
    }

    public void TradeState(EntityId entityId, TradeStatus state)
    {
        packageSender.ToPlayer(entityId, new TradeStatePacket { State = (byte)state });
    }

    public void TradeOffer(EntityId entityId, bool own = true)
    {
        var entity = entities.Get(entityId)!;
        var trade = entity.Get<TradeState>()!;
        var toId = own ? entityId : trade.Partner!.Value;
        var toEntity = entities.Get(toId)!;
        var toInv = toEntity.Get<InventoryState>()!;
        var toTrade = toEntity.Get<TradeState>()!;
        var packet = new TradeOfferPacket
        {
            Own = own,
            Items = new PacketsTradeOfferItem[MaxInventory]
        };
        for (short i = 0; i < MaxInventory; i++)
        {
            packet.Items[i] = new PacketsTradeOfferItem
            {
                ItemId = toInv.Slots[toTrade.Offer![i].SlotNum].ItemId,
                Amount = toTrade.Offer[i].Amount
            };
        }
        packageSender.ToPlayer(entityId, packet);
    }
}
