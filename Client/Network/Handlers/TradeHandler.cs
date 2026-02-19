using CryBits.Client.Entities;
using CryBits.Client.Framework;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Network.Senders;
using CryBits.Client.UI.Events;
using CryBits.Entities;
using CryBits.Entities.Slots;
using CryBits.Enums;
using CryBits.Extensions;
using LiteNetLib.Utils;
using static CryBits.Globals;

namespace CryBits.Client.Network.Handlers;

internal static class TradeHandler
{
    internal static void Trade(NetDataReader data)
    {
        var state = data.GetBool();

        // Set trade panel visibility
        Panels.Trade.Visible = data.GetBool();

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

    internal static void TradeInvitation(NetDataReader data)
    {
        // Decline if player disabled trade invitations
        if (!Options.Trade)
        {
            TradeSender.TradeDecline();
            return;
        }

        // Show trade invitation panel
        PanelsEvents.TradeInvitation = data.GetString();
        Panels.TradeInvitation.Visible = true;
    }

    internal static void TradeState(NetDataReader data)
    {
        switch ((TradeStatus)data.GetByte())
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

    internal static void TradeOffer(NetDataReader data)
    {
        // Read trade offer data
        if (data.GetBool())
            for (byte i = 0; i < MaxInventory; i++)
            {
                Player.Me.TradeOffer[i].Item = Item.List.Get(data.GetGuid());
                Player.Me.TradeOffer[i].Amount = data.GetShort();
            }
        else
            for (byte i = 0; i < MaxInventory; i++)
            {
                Player.Me.TradeTheirOffer[i].Item = Item.List.Get(data.GetGuid());
                Player.Me.TradeTheirOffer[i].Amount = data.GetShort();
            }
    }
}
