using CryBits.Client.Framework;
using CryBits.Client.Network.Senders;
using CryBits.Client.UI.Game;
using CryBits.Client.Worlds;
using CryBits.Protocol;
using CryBits.Protocol.Packets.Server;
using CryBits.Simulation.Components;
using CryBits.Simulation.Intents;
using CryBits.Simulation.State;

namespace CryBits.Client.Network.Handlers;

internal class PartyHandler(IntentSender intentSender, GameContext context, GameScreen gameScreen)
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
        if (!Options.Instance.Party)
        {
            intentSender.Send(new PartyDeclineIntent(default));
            return;
        }

        gameScreen.PartyInvitationView.Show(packet.PlayerInvitation);
    }
}
