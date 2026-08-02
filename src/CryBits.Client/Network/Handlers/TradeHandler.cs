using CryBits.Client.Framework;
using CryBits.Client.Network.Senders;
using CryBits.Client.UI.Game;
using CryBits.Client.UI.Game.ViewModels;
using CryBits.Definitions.Common;
using CryBits.Protocol;
using CryBits.Protocol.Packets.Server;
using CryBits.Simulation.Intents;

namespace CryBits.Client.Network.Handlers;

internal class TradeHandler(IntentSender intentSender, GameScreen gameScreen, TradeViewModel viewModel)
{
    [PacketHandler]
    internal void Trade(TradePacket packet)
    {
        gameScreen.TradeView.Open(packet.State);
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
        gameScreen.TradeView.SetStatus((TradeStatus)packet.State);
    }

    [PacketHandler]
    internal void TradeOffer(TradeOfferPacket packet)
    {
        if (packet.Own)
            viewModel.UpdateOwnOffer(packet.Items);
        else
            viewModel.UpdateTheirOffer(packet.Items);
    }
}
