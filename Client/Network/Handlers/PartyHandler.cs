using System.Linq;
using CryBits.Client.ECS;
using CryBits.Client.ECS.Components;
using CryBits.Client.Framework;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Network.Senders;
using CryBits.Client.UI.Events;
using CryBits.Packets.Server;

namespace CryBits.Client.Network.Handlers;

internal static class PartyHandler
{
    private static GameContext Ctx => GameContext.Instance;

    [PacketHandler]
    internal static void Party(PartyPacket packet)
    {
        var localId = Ctx.GetLocalPlayer();
        if (localId < 0) return;

        // Resolve each member name to an entity id.
        var memberIds = packet.Members
            .Select(name => Ctx.FindOrCreatePlayer(name))
            .ToArray();

        if (!Ctx.World.Has<PartyComponent>(localId))
            Ctx.World.Add(localId, new PartyComponent());

        Ctx.World.Get<PartyComponent>(localId).MemberEntityIds = memberIds;
    }

    [PacketHandler]
    internal static void PartyInvitation(PartyInvitationPacket packet)
    {
        if (!Options.Party)
        {
            PartySender.PartyDecline();
            return;
        }

        PanelsEvents.PartyInvitation = packet.PlayerInvitation;
        Panels.PartyInvitation.Visible = true;
    }
}
