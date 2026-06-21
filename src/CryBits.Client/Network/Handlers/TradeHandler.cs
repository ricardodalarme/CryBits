using CryBits.Client.Framework;
using CryBits.Client.Network.Senders;
using CryBits.Client.UI.Game.Views;
using CryBits.Client.Worlds;
using CryBits.Definitions.Common;
using CryBits.Definitions.Slots;
using CryBits.Protocol;
using CryBits.Protocol.Packets.Server;
using CryBits.Simulation.Components;
using CryBits.Simulation.Intents;
using static CryBits.Definitions.Globals;

namespace CryBits.Client.Network.Handlers;

internal class TradeHandler(IntentSender intentSender, GameContext context)
{
    [PacketHandler]
    internal void Trade(TradePacket packet)
    {
        var playerEntity = context.LocalPlayer.Entity;
        if (playerEntity is null) return;
        var entity = playerEntity.Value;

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
            context.World.Set(entity, new TradeState());
        }
        else
        {
            // Detach trade state — removal is the reset; no leftover data.
            context.World.Remove<TradeState>(entity);
        }
    }

    [PacketHandler]
    internal void TradeInvitation(TradeInvitationPacket packet)
    {
        // Decline if player disabled trade invitations
        if (!Options.Instance.Trade)
        {
            intentSender.Send(new TradeDeclineIntent(default));
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
        var trade = context.LocalPlayer.GetTrade();
        if (trade == null) return;
        if (packet.Own)
        {
            if (trade.Offer == null) trade.Offer = new TradeSlot[MaxInventory];
            for (byte i = 0; i < MaxInventory && i < trade.Offer.Length; i++)
                trade.Offer[i] = new TradeSlot { SlotNum = (short)i, Amount = packet.Items[i].Amount };
        }
        else
        {
            if (trade.TheirOffer == null) trade.TheirOffer = new TradeSlot[MaxInventory];
            for (byte i = 0; i < MaxInventory && i < trade.TheirOffer.Length; i++)
                trade.TheirOffer[i] = new TradeSlot { SlotNum = (short)i, Amount = packet.Items[i].Amount };
        }
    }
}
