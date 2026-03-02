using CryBits.Client.Entities;
using CryBits.Client.Framework;
using CryBits.Client.Network.Senders;
using CryBits.Client.UI.Game.Views;
using CryBits.Entities;
using CryBits.Entities.Slots;
using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Packets.Server;
using static CryBits.Globals;

namespace CryBits.Client.Network.Handlers;

internal class TradeHandler(TradeSender tradeSender)
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

            // Clear trade offer data
            Player.Me.TradeOffer = new ItemSlot[MaxInventory];
            Player.Me.TradeTheirOffer = new ItemSlot[MaxInventory];
        }
        else
        {
            // Clear trade offer data
            Player.Me.TradeOffer = null;
            Player.Me.TradeTheirOffer = null;
        }
    }

    [PacketHandler]
    internal void TradeInvitation(TradeInvitationPacket packet)
    {
        // Decline if player disabled trade invitations
        if (!Options.Trade)
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
        if (packet.Own)
            for (byte i = 0; i < MaxInventory; i++)
            {
                Player.Me.TradeOffer[i].Item = Item.List.Get(packet.Items[i].ItemId);
                Player.Me.TradeOffer[i].Amount = packet.Items[i].Amount;
            }
        else
            for (byte i = 0; i < MaxInventory; i++)
            {
                Player.Me.TradeTheirOffer[i].Item = Item.List.Get(packet.Items[i].ItemId);
                Player.Me.TradeTheirOffer[i].Amount = packet.Items[i].Amount;
            }
    }
}
