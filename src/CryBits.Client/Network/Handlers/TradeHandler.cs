using CryBits.Client.Components.Trade;
using CryBits.Client.Framework;
using CryBits.Client.Network.Senders;
using CryBits.Client.UI.Game.Views;
using CryBits.Client.Worlds;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Common;
using CryBits.Definitions.Slots;
using CryBits.Transport;
using CryBits.Transport.Packets.Server;
using static CryBits.Definitions.Globals;

namespace CryBits.Client.Network.Handlers;

internal class TradeHandler(TradeSender tradeSender, GameContext context, DefinitionCatalog catalog)
{
    private readonly DefinitionCatalog _catalog = catalog;
    [PacketHandler]
    internal void Trade(TradePacket packet)
    {
        var state = packet.State;

        // Set trade panel visibility
        TradeView.PanelVisible = packet.State;

        if (state)
        {
            // Reset trade buttons
            TradeView.ConfirmOfferButtonVisible = true;
            TradeAmountView.PanelVisible = TradeView.AcceptOfferButtonVisible = TradeView.DeclineOfferButtonVisible = false;
            TradeView.OfferDisabledPanelVisible = false;

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
                TradeView.ConfirmOfferButtonVisible = true;
                TradeView.AcceptOfferButtonVisible = TradeView.DeclineOfferButtonVisible = false;
                TradeView.OfferDisabledPanelVisible = false;
                break;
            case TradeStatus.Confirmed:
                TradeView.ConfirmOfferButtonVisible = false;
                TradeView.AcceptOfferButtonVisible = TradeView.DeclineOfferButtonVisible = true;
                TradeView.OfferDisabledPanelVisible = false;
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
                trade.Offer[i] = new ItemSlot(packet.Items[i].ItemId, packet.Items[i].Amount);
        else
            for (byte i = 0; i < MaxInventory; i++)
                trade.TheirOffer[i] = new ItemSlot(packet.Items[i].ItemId, packet.Items[i].Amount);
    }
}
