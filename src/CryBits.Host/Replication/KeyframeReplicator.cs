using CryBits.Definitions.Maps;
using CryBits.Host.Core;
using CryBits.Protocol.Packets.Server;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Transport;
using CryBits.Transport.Abstractions;
using MemoryPack;

namespace CryBits.Host.Replication;

internal sealed class KeyframeReplicator(
    World world,
    SessionManager sessions,
    KeyframeEncoder encoder,
    EventFanout eventFanout,
    ITransport transport,
    InterestManager interestManager) : ISimulationSystem
{
    public void Execute(World world, Tick tick)
    {
        if (world.Dirty == null) return;

        var hasDirty = world.Dirty.All.Count > 0;
        var hasEvents = tick.Events.Events.Count > 0;

        if (!hasDirty && !hasEvents) return;

        if (hasDirty) SendKeyframes();
        if (hasEvents) eventFanout.Fanout(tick, world);
        world.Dirty.Clear();
    }

    private void SendKeyframes()
    {
        var dirtyMaps = new HashSet<Guid>();
        foreach (var (entityId, _) in world.Dirty!.All)
        {
            var pos = world.Get<Position>(entityId);
            if (pos != null) dirtyMaps.Add(pos.MapId);
        }

        foreach (var session in sessions.Where(s => s.IsPlaying && s.Character.HasValue))
        {
            var observerId = session.Character!.Value;
            var pos = world.Get<Position>(observerId);
            if (pos == null) continue;

            if (!dirtyMaps.Contains(pos.MapId)) continue;

            var diff = interestManager.Update(observerId);

            // Send chunk payloads for newly entered chunks
            foreach (var chunk in diff.Entered)
            {
                var payload = BuildChunkPayload(pos.MapId, chunk);
                if (payload != null)
                {
                    var chunkBytes = MemoryPackSerializer.Serialize<IServerPacket>(payload);
                    transport.Send(session.Id, chunkBytes, DeliveryChannel.ReliableOrdered);
                }
            }

            // Notify client to evict chunks that left AOI
            foreach (var chunk in diff.Left)
            {
                var evict = new ChunkRevisionPacket
                {
                    MapId = pos.MapId,
                    ChunkX = chunk.X,
                    ChunkY = chunk.Y,
                    Version = -1
                };
                var evictBytes = MemoryPackSerializer.Serialize<IServerPacket>(evict);
                transport.Send(session.Id, evictBytes, DeliveryChannel.ReliableOrdered);
            }

            // Send keyframe for observable entities
            var visible = interestManager.GetObservableEntities(observerId).ToList();
            if (visible.Count == 0) continue;

            var packet = encoder.Encode(pos.MapId, visible);
            var keyframeBytes = MemoryPackSerializer.Serialize<IServerPacket>(packet);
            transport.Send(session.Id, keyframeBytes, DeliveryChannel.ReliableOrdered);
        }
    }

    private ChunkPayload? BuildChunkPayload(Guid mapId, ChunkCoord chunk) =>
        ChunkPayloadBuilder.Build(world, mapId, chunk.X, chunk.Y);
}
