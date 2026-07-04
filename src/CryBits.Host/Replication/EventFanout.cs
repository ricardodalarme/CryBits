using CryBits.Host.Core;
using CryBits.Host.Network.Senders;
using CryBits.Protocol.Packets.Server;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Events;
using CryBits.Simulation.Spatial;
using CryBits.Transport;
using CryBits.Transport.Abstractions;
using MemoryPack;

namespace CryBits.Host.Replication;

internal sealed class EventFanout(
    SessionManager sessions,
    ChatSender chatSender,
    ContentSender contentSender,
    ITransport transport)
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
        foreach (var map in world.MapDefs.Values)
        {
            if (map.Id == warp.NewMapId)
            {
                var session = sessions.Get(warp.PlayerId);
                if (session == null) return;

                contentSender.Map(session, map.Id);
                contentSender.MapRevision(session, map.Id);

                // Send initial AOI chunks for the new map position
                var pos = world.Get<Position>(warp.PlayerId);
                if (pos != null)
                {
                    var center = ChunkGrid.FromPosition(pos.X, pos.Y);
                    foreach (var chunkCoord in world.SpatialGrid.GetNeighborhood(center, 2))
                    {
                        var payload = ChunkPayloadBuilder.Build(world, pos.MapId, chunkCoord.X, chunkCoord.Y);
                        if (payload != null)
                        {
                            var bytes = MemoryPackSerializer.Serialize<IServerPacket>(payload);
                            transport.Send(session.Id, bytes, DeliveryChannel.ReliableOrdered);
                        }
                    }
                }

                break;
            }
        }
    }
}
