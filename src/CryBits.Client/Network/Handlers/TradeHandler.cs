using CryBits.Client.Framework;
using CryBits.Client.Network.Senders;
using CryBits.Client.Worlds;
using CryBits.Definitions.Common;
using CryBits.Definitions.Slots;
using CryBits.Protocol;
using CryBits.Protocol.Packets.Server;
using CryBits.Simulation.Components;
using CryBits.Simulation.Intents;
using static CryBits.Definitions.Globals;
using CryBits.Client.UI.Game;

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
        GameScreen.Instance.TradeView.Panel.Visible = packet.State;

        if (state)
        {
            // Reset trade buttons
            GameScreen.Instance.TradeView.ConfirmOfferButton.Visible = true;
            GameScreen.Instance.TradeAmountView.Panel.Visible = GameScreen.Instance.TradeView.AcceptOfferButton.Visible = GameScreen.Instance.TradeView.DeclineOfferButton.Visible = false;
            GameScreen.Instance.TradeView.OfferDisabledPanel.Visible = false;

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
        GameScreen.Instance.TradeInvitationView.Show(packet.PlayerInvitation);
    }

    [PacketHandler]
    internal void TradeState(TradeStatePacket packet)
    {
        switch ((TradeStatus)packet.State)
        {
            case TradeStatus.Accepted:
            case TradeStatus.Declined:
                GameScreen.Instance.TradeView.ConfirmOfferButton.Visible = true;
                GameScreen.Instance.TradeView.AcceptOfferButton.Visible = GameScreen.Instance.TradeView.DeclineOfferButton.Visible = false;
                GameScreen.Instance.TradeView.OfferDisabledPanel.Visible = false;
                break;
            case TradeStatus.Confirmed:
                GameScreen.Instance.TradeView.ConfirmOfferButton.Visible = false;
                GameScreen.Instance.TradeView.AcceptOfferButton.Visible = GameScreen.Instance.TradeView.DeclineOfferButton.Visible = true;
                GameScreen.Instance.TradeView.OfferDisabledPanel.Visible = false;
                break;
        }
    }

    [PacketHandler]
    internal void TradeOffer(TradeOfferPacket packet)
    {
        var trade = context.LocalPlayer.GetTrade();
        if (trade == null) return;
        var newOffer = new TradeSlot[MaxInventory];
        if (packet.Own)
        {
            for (byte i = 0; i < MaxInventory; i++)
                newOffer[i] = new TradeSlot { SlotNum = (short)i, Amount = packet.Items[i].Amount };
            context.World.Set(context.LocalPlayer.Entity!.Value, trade with { Offer = newOffer });
        }
        else
        {
            for (byte i = 0; i < MaxInventory; i++)
                newOffer[i] = new TradeSlot { SlotNum = (short)i, Amount = packet.Items[i].Amount };
            context.World.Set(context.LocalPlayer.Entity!.Value, trade with { TheirOffer = newOffer });
        }
    }
}
