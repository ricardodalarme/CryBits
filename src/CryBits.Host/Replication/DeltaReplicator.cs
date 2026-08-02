using CryBits.Definitions.Maps;
using CryBits.Host.Core;
using CryBits.Protocol.Packets.Server;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Systems;
using CryBits.Transport;
using CryBits.Transport.Abstractions;
using MemoryPack;

namespace CryBits.Host.Replication;

internal sealed class DeltaReplicator(
    World world,
    SessionManager sessions,
    DeltaEncoder deltaEncoder,
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

        if (hasDirty) Replicate(tick.TickNumber);
        if (hasEvents) eventFanout.Fanout(tick, world);
        world.Dirty.Clear();
    }

    private void Replicate(long tickNumber)
    {
        var dirtyMaps = CollectDirtyMaps();

        foreach (var session in sessions.Where(s => s.IsPlaying && s.Character.HasValue))
        {
            var observerId = session.Character!.Value;
            var pos = world.Get<Position>(observerId);
            if (pos == null) continue;

            if (!dirtyMaps.Contains(pos.MapId)) continue;

            var observer = session.ReplicationState;
            if (observer == null) continue;

            var diff = interestManager.Update(observerId);
            foreach (var chunk in diff.Entered)
                SendChunkPayload(session, pos.MapId, chunk);
            foreach (var chunk in diff.Left)
                SendChunkEviction(session, pos.MapId, chunk);

            var visible = interestManager.GetObservableEntities(observerId).ToList();
            if (visible.Count == 0) continue;

            SendDelta(session, observer, pos.MapId, visible, tickNumber);
        }
    }

    private void SendDelta(Session session, ObserverState observer, Guid mapId,
        List<EntityId> visible, long tickNumber)
    {
        var delta = deltaEncoder.EncodeDelta(mapId, tickNumber, observer, visible);
        if (delta == null) return;

        var bytes = MemoryPackSerializer.Serialize<IServerPacket>(delta);
        transport.Send(session.Id, bytes, DeliveryChannel.Sequenced);
    }

    private HashSet<Guid> CollectDirtyMaps()
    {
        var dirtyMaps = new HashSet<Guid>();
        foreach (var entityId in world.Dirty!.All)
        {
            var pos = world.Get<Position>(entityId);
            if (pos != null) dirtyMaps.Add(pos.MapId);
        }

        return dirtyMaps;
    }

    private void SendChunkPayload(Session session, Guid mapId, ChunkCoord chunk)
    {
        var payload = ChunkPayloadBuilder.Build(world, mapId, chunk.X, chunk.Y);
        if (payload != null)
        {
            var chunkBytes = MemoryPackSerializer.Serialize<IServerPacket>(payload);
            transport.Send(session.Id, chunkBytes, DeliveryChannel.ReliableOrdered);
        }
    }

    private void SendChunkEviction(Session session, Guid mapId, ChunkCoord chunk)
    {
        var evict = new ChunkRevisionPacket { MapId = mapId, ChunkX = chunk.X, ChunkY = chunk.Y, Version = -1 };
        var evictBytes = MemoryPackSerializer.Serialize<IServerPacket>(evict);
        transport.Send(session.Id, evictBytes, DeliveryChannel.ReliableOrdered);
    }
}
