using CryBits.Client.Components.Trade;
using CryBits.Client.Framework;
using CryBits.Client.Network.Senders;
using CryBits.Client.UI.Game.Views;
using CryBits.Client.Worlds;
using CryBits.Entities;
using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Packets.Server;
using static CryBits.Globals;

namespace CryBits.Client.Network.Handlers;

internal class TradeHandler(TradeSender tradeSender, GameContext context)
{
    [PacketHandler]
    internal void Trade(TradePacket packet)
    {
        var state = packet.State;

        // Set trade panel visibility
        TradeView.Panel.Visible = packet.State;

        if (state)
        {
            // Reset trade buttons
            TradeView.ConfirmOfferButton.Visible = true;
            TradeAmountView.Panel.Visible = TradeView.AcceptOfferButton.Visible = TradeView.DeclineOfferButton.Visible = false;
            TradeView.OfferDisabledPanel.Visible = false;

            // Attach fresh trade state to the local player entity for the duration of this session.
            context.World.Add(context.LocalPlayer.Entity, new TradeComponent());
        }
        else
        {
            // Detach trade state — removal is the reset; no leftover data.
            context.World.Remove<TradeComponent>(context.LocalPlayer.Entity);
        }
    }

    [PacketHandler]
    internal void TradeInvitation(TradeInvitationPacket packet)
    {
        // Decline if player disabled trade invitations
        if (!Options.Instance.Trade)
        {
            tradeSender.TradeDecline();
            return;
        }

        // Show trade invitation panel
        TradeInvitationView.Show(packet.PlayerInvitation);
    }

    [PacketHandler]
    internal void TradeState(TradeStatePacket packet)
    {
        switch ((TradeStatus)packet.State)
        {
            case TradeStatus.Accepted:
            case TradeStatus.Declined:
                TradeView.ConfirmOfferButton.Visible = true;
                TradeView.AcceptOfferButton.Visible = TradeView.DeclineOfferButton.Visible = false;
                TradeView.OfferDisabledPanel.Visible = false;
                break;
            case TradeStatus.Confirmed:
                TradeView.ConfirmOfferButton.Visible = false;
                TradeView.AcceptOfferButton.Visible = TradeView.DeclineOfferButton.Visible = true;
                TradeView.OfferDisabledPanel.Visible = false;
                break;
        }
    }

    [PacketHandler]
    internal void TradeOffer(TradeOfferPacket packet)
    {
        // Read trade offer data
        ref var trade = ref context.LocalPlayer.GetTrade();
        if (packet.Own)
            for (byte i = 0; i < MaxInventory; i++)
            {
                trade.Offer[i].Item = Item.List.Get(packet.Items[i].ItemId);
                trade.Offer[i].Amount = packet.Items[i].Amount;
            }
        else
            for (byte i = 0; i < MaxInventory; i++)
            {
                trade.TheirOffer[i].Item = Item.List.Get(packet.Items[i].ItemId);
                trade.TheirOffer[i].Amount = packet.Items[i].Amount;
            }
    }
}
