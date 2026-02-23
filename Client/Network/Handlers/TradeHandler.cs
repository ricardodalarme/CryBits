using System.Collections.Generic;
using CryBits.Client.ECS;
using CryBits.Client.ECS.Components;
using CryBits.Client.Framework;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Network.Senders;
using CryBits.Client.UI.Events;
using CryBits.Entities;
using CryBits.Entities.Slots;
using CryBits.Enums;
using CryBits.Packets.Server;
using static CryBits.Globals;

namespace CryBits.Client.Network.Handlers;

internal static class TradeHandler
{
    private static GameContext Ctx => GameContext.Instance;

    [PacketHandler]
    internal static void Trade(TradePacket packet)
    {
        var state = packet.State;
        var localId = Ctx.GetLocalPlayer();

        Panels.Trade.Visible = state;

        if (localId < 0) return;

        if (state)
        {
            Buttons.TradeOfferConfirm.Visible = true;
            Panels.TradeAmount.Visible = Buttons.TradeOfferAccept.Visible = Buttons.TradeOfferDecline.Visible = false;
            Panels.TradeOfferDisable.Visible = false;

            // Attach trade component with empty offer slots.
            Ctx.World.Add(localId, new TradeComponent
            {
                Offer = new ItemSlot[MaxInventory],
                TheirOffer = new ItemSlot[MaxInventory]
            });
        }
        else
        {
            Ctx.World.Remove<TradeComponent>(localId);
        }
    }

    [PacketHandler]
    internal static void TradeInvitation(TradeInvitationPacket packet)
    {
        if (!Options.Trade)
        {
            TradeSender.TradeDecline();
            return;
        }

        PanelsEvents.TradeInvitation = packet.PlayerInvitation;
        Panels.TradeInvitation.Visible = true;
    }

    [PacketHandler]
    internal static void TradeState(TradeStatePacket packet)
    {
        switch ((TradeStatus)packet.State)
        {
            case TradeStatus.Accepted:
            case TradeStatus.Declined:
                Buttons.TradeOfferConfirm.Visible = true;
                Buttons.TradeOfferAccept.Visible = Buttons.TradeOfferDecline.Visible = false;
                Panels.TradeOfferDisable.Visible = false;
                break;
            case TradeStatus.Confirmed:
                Buttons.TradeOfferConfirm.Visible = false;
                Buttons.TradeOfferAccept.Visible = Buttons.TradeOfferDecline.Visible = true;
                Panels.TradeOfferDisable.Visible = false;
                break;
        }
    }

    [PacketHandler]
    internal static void TradeOffer(TradeOfferPacket packet)
    {
        var localId = Ctx.GetLocalPlayer();
        if (localId < 0) return;
        if (!Ctx.World.TryGet<TradeComponent>(localId, out var trade)) return;

        var target = packet.Own ? trade.Offer : trade.TheirOffer;
        if (target == null) return;

        for (byte i = 0; i < MaxInventory; i++)
        {
            target[i].Item = Item.List.GetValueOrDefault(packet.Items[i].ItemId);
            target[i].Amount = packet.Items[i].Amount;
        }
    }
}
