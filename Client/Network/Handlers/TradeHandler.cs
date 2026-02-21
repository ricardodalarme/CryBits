using CryBits.Client.Entities;
using CryBits.Client.Framework;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Network.Senders;
using CryBits.Client.UI.Events;
using CryBits.Entities;
using CryBits.Entities.Slots;
using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Packets.Server;
using static CryBits.Globals;

namespace CryBits.Client.Network.Handlers;

internal static class TradeHandler
{
    internal static void Trade(TradePacket packet)
    {
        var state = packet.State;

        // Set trade panel visibility
        Panels.Trade.Visible = packet.State;

        if (state)
        {
            // Reset trade buttons
            Buttons.TradeOfferConfirm.Visible = true;
            Panels.TradeAmount.Visible = Buttons.TradeOfferAccept.Visible = Buttons.TradeOfferDecline.Visible = false;
            Panels.TradeOfferDisable.Visible = false;

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

    internal static void TradeInvitation(TradeInvitationPacket packet)
    {
        // Decline if player disabled trade invitations
        if (!Options.Trade)
        {
            TradeSender.TradeDecline();
            return;
        }

        // Show trade invitation panel
        PanelsEvents.TradeInvitation = packet.PlayerInvitation;
        Panels.TradeInvitation.Visible = true;
    }

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

    internal static void TradeOffer(TradeOfferPacket packet)
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
