using CryBits.Host.Core;
using CryBits.Host.Network.Senders;
using CryBits.Simulation.Core;
using CryBits.Simulation.Events;

namespace CryBits.Host.Replication;

internal sealed class EventFanout(
    SessionManager sessions,
    ChatSender chatSender,
    ContentSender contentSender)
{
    public void Fanout(Tick tick, World world)
    {
        foreach (var evt in tick.Events.Events)
        {
            if (evt is PlayerWarpedEvent warp && warp.NeedsMapData)
                ReplicatePlayerWarp(warp, world);

            if (evt is ChatMessageEvent chat)
            {
                var session = sessions.Get(chat.RecipientId);
                if (session != null)
                    chatSender.SendMessage(session, chat.Text, chat.ColorArgb);
            }
        }
    }

    private void ReplicatePlayerWarp(PlayerWarpedEvent warp, World world)
    {
        foreach (var map in world.Maps.Values)
        {
            if (map.Id == warp.NewMapId)
            {
                var session = sessions.Get(warp.PlayerId);
                if (session != null)
                {
                    contentSender.MapRevision(session, map.Id);
                }
                break;
            }
        }
    }
}
