using CryBits.Host.Services.Party;
using CryBits.Host.Services.Trade;
using CryBits.Protocol;
using CryBits.Protocol.Packets.Client;
using CryBits.Protocol.Serialization;
using CryBits.Simulation.Core;
using CryBits.Simulation.Intents;
using MemoryPack;
using Microsoft.Extensions.Logging;
using ZLogger;

namespace CryBits.Host.Ingress;

public sealed class IntentIngress(
    IntentFunnel funnel,
    TradeService tradeService,
    PartyService partyService,
    ILogger<IntentIngress> logger)
{
    [PacketHandler]
    public void Handle(EntityId entityId, IntentPacket packet)
    {
        var intentType = IntentRegistry.GetTypeForTag(packet.IntentTag);
        if (intentType is null)
        {
            logger.ZLogWarning($"Unknown intent tag {packet.IntentTag} from entity {entityId.Value}");
            return;
        }

        if (MemoryPackSerializer.Deserialize(intentType, packet.Data) is Intent intent)
        {
            if (intent is TradeInviteIntent invite)
                tradeService.HandleInvite(entityId, invite.PlayerName);
            else if (intent is TradeAcceptIntent)
                tradeService.HandleAccept(entityId);
            else if (intent is TradeDeclineIntent)
                tradeService.HandleDecline(entityId);
            else if (intent is TradeLeaveIntent)
                tradeService.HandleLeave(entityId);
            else if (intent is TradeOfferIntent offer)
                tradeService.HandleOffer(entityId, offer.OfferSlot, offer.InventorySlot, offer.Amount);
            else if (intent is TradeOfferStateIntent offerState)
                tradeService.HandleOfferState(entityId, offerState.State);
            else if (intent is PartyInviteIntent partyInvite)
                partyService.HandleInvite(entityId, partyInvite.PlayerName);
            else if (intent is PartyAcceptIntent)
                partyService.HandleAccept(entityId);
            else if (intent is PartyDeclineIntent)
                partyService.HandleDecline(entityId);
            else if (intent is PartyLeaveIntent)
                partyService.HandleLeave(entityId);
            else
                funnel.Submit(intent with { SourceEntityId = entityId });
        }
        else
        {
            logger.ZLogError($"Failed to deserialize intent tag {packet.IntentTag} from entity {entityId.Value}");
        }
    }
}
