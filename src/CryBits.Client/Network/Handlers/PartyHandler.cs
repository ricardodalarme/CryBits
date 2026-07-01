using CryBits.Client.Framework;
using CryBits.Client.UI.Game;
using CryBits.Client.Network.Senders;
using CryBits.Client.Worlds;
using CryBits.Protocol;
using CryBits.Protocol.Packets.Server;
using CryBits.Simulation.Components;
using CryBits.Simulation.Intents;
using CryBits.Simulation.State;

namespace CryBits.Client.Network.Handlers;

internal class PartyHandler(IntentSender intentSender, GameContext context)
{
    [PacketHandler]
    internal void Party(PartyPacket packet)
    {
        var entityNullable = context.LocalPlayer.Entity;
        if (entityNullable is null) return;
        var entity = entityNullable.Value;
        var world = context.World;

        if (packet.MemberIds.Length == 0)
        {
            // No members — party disbanded or player left; drop the component.
            if (world.Has<PartyState>(entity))
                world.Remove<PartyState>(entity);
            return;
        }

        var members = new List<EntityId>(packet.MemberIds.Length);
        for (byte i = 0; i < packet.MemberIds.Length; i++)
            members.Add(new EntityId(packet.MemberIds[i]));
        world.Set(entity, new PartyState(members, null));
    }

    [PacketHandler]
    internal void PartyInvitation(PartyInvitationPacket packet)
    {
        // Decline if player disabled party invites
        if (!Options.Instance.Party)
        {
            intentSender.Send(new PartyDeclineIntent(default));
            return;
        }

        // Show party invitation panel
        GameScreen.Instance.PartyInvitationView.Show(packet.PlayerInvitation);
    }
}
