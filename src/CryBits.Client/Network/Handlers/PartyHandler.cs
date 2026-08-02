using CryBits.Client.Framework;
using CryBits.Client.Network.Senders;
using CryBits.Client.UI.Game;
using CryBits.Client.UI.Game.ViewModels;
using CryBits.Protocol;
using CryBits.Protocol.Packets.Server;
using CryBits.Simulation.Intents;

namespace CryBits.Client.Network.Handlers;

internal class PartyHandler(IntentSender intentSender, GameScreen gameScreen, PartyViewModel viewModel)
{
    [PacketHandler]
    internal void Party(PartyPacket packet)
    {
        if (packet.MemberIds.Length == 0)
        {
            viewModel.Members = [];
            return;
        }

        var list = new List<PartyMemberViewModel>(packet.MemberIds.Length);
        for (byte i = 0; i < packet.MemberIds.Length; i++) list.Add(new PartyMemberViewModel { Id = packet.MemberIds[i] });

        viewModel.Members = list;
    }

    [PacketHandler]
    internal void PartyInvitation(PartyInvitationPacket packet)
    {
        if (!Options.Instance.Party)
        {
            intentSender.Send(new PartyDeclineIntent(default));
            return;
        }

        gameScreen.PartyInvitationView.Show(packet.PlayerInvitation);
    }
}
