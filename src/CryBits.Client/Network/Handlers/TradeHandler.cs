using CryBits.Client.Core;
using CryBits.Client.Framework;
using CryBits.Client.Network.Senders;
using CryBits.Client.UI.Game;
using CryBits.Client.UI.Game.ViewModels;
using CryBits.Definitions.Common;
using CryBits.Definitions.Slots;
using CryBits.Protocol;
using CryBits.Protocol.Packets.Server;
using CryBits.Simulation.Intents;
using static CryBits.Definitions.Globals;

namespace CryBits.Client.Network.Handlers;

internal class TradeHandler(IntentSender intentSender, GameScreen gameScreen, TradeViewModel viewModel)
{
    [PacketHandler]
    internal void Trade(TradePacket packet)
    {
        var state = packet.State;

        gameScreen.TradeView.Panel.Visible = packet.State;
        viewModel.IsOpen = packet.State;

        if (state)
        {
            gameScreen.TradeView.ConfirmOfferButton.Visible = true;
            gameScreen.TradeAmountView.Panel.Visible = gameScreen.TradeView.AcceptOfferButton.Visible = gameScreen.TradeView.DeclineOfferButton.Visible = false;
            gameScreen.TradeView.OfferDisabledPanel.Visible = false;

            viewModel.OwnOffer = new TradeSlot[MaxInventory];
            viewModel.TheirOffer = new TradeSlot[MaxInventory];
        }
    }

    [PacketHandler]
    internal void TradeInvitation(TradeInvitationPacket packet)
    {
        if (!Options.Instance.Trade)
        {
            intentSender.Send(new TradeDeclineIntent(default));
            return;
        }

        gameScreen.TradeInvitationView.Show(packet.PlayerInvitation);
    }

    [PacketHandler]
    internal void TradeState(TradeStatePacket packet)
    {
        switch ((TradeStatus)packet.State)
        {
            case TradeStatus.Accepted:
            case TradeStatus.Declined:
                gameScreen.TradeView.ConfirmOfferButton.Visible = true;
                gameScreen.TradeView.AcceptOfferButton.Visible = gameScreen.TradeView.DeclineOfferButton.Visible = false;
                gameScreen.TradeView.OfferDisabledPanel.Visible = false;
                break;
            case TradeStatus.Confirmed:
                gameScreen.TradeView.ConfirmOfferButton.Visible = false;
                gameScreen.TradeView.AcceptOfferButton.Visible = gameScreen.TradeView.DeclineOfferButton.Visible = true;
                gameScreen.TradeView.OfferDisabledPanel.Visible = false;
                break;
        }
    }

    [PacketHandler]
    internal void TradeOffer(TradeOfferPacket packet)
    {
        var newOffer = new TradeSlot[MaxInventory];
        for (byte i = 0; i < MaxInventory; i++)
            newOffer[i] = new TradeSlot { SlotNum = (short)i, Amount = packet.Items[i].Amount };

        if (packet.Own)
        {
            viewModel.OwnOffer = newOffer;
        }
        else
        {
            viewModel.TheirOffer = newOffer;
        }
    }
}
