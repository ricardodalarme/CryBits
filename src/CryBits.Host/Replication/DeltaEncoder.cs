using CryBits.Host.Core;
using CryBits.Protocol.Packets;
using CryBits.Protocol.Packets.Server;
using CryBits.Protocol.Serialization;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.State;
using MemoryPack;

namespace CryBits.Host.Replication;

internal sealed class DeltaEncoder(World world)
{
    public DeltaPacket? EncodeDelta(
        Guid mapId,
        long currentTick,
        ObserverState observer,
        IEnumerable<EntityId> visibleEntities)
    {
        var packet = new DeltaPacket
        {
            TickNumber = currentTick,
            BaselineTick = observer.LastAckedTick,
            MapId = mapId
        };

        var visibleIds = new HashSet<long>();

        foreach (var entityId in visibleEntities)
        {
            var state = world.Entities.Get(entityId);
            if (state == null) continue;

            var serverId = entityId.Value;
            visibleIds.Add(serverId);

            var isAdded = observer.KnownEntities.Add(serverId);
            var delta = new EntityDelta
            {
                EntityId = serverId,
                Kind = DetermineKind(state),
                Action = isAdded ? DeltaAction.Added : DeltaAction.Changed
            };

            foreach (var (type, obj) in state.GetAllComponents())
            {
                var tag = ComponentTypeRegistry.Tag(type);
                if (tag == null) continue;

                if (!isAdded && state.GetVersion(type) <= observer.LastAckedTick)
                    continue;

                var data = MemoryPackSerializer.Serialize(type, obj);
                delta.Components.Add(new ComponentData { Tag = tag.Value, Data = data });
            }

            if (!isAdded)
            {
                foreach (var (type, removalVersion) in state.GetRemovals())
                {
                    if (removalVersion <= observer.LastAckedTick) continue;
                    var tag = ComponentTypeRegistry.Tag(type);
                    if (tag == null) continue;
                    delta.RemovedTags.Add(tag.Value);
                }
            }

            if (delta.Components.Count > 0 || delta.RemovedTags.Count > 0)
                packet.Entities.Add(delta);
        }

        foreach (var knownId in observer.KnownEntities.ToList())
        {
            if (!visibleIds.Contains(knownId))
            {
                packet.RemovedEntities.Add(knownId);
                observer.KnownEntities.Remove(knownId);
            }
        }

        return packet.Entities.Count > 0 || packet.RemovedEntities.Count > 0 ? packet : null;
    }

    public KeyframePacket EncodeKeyframe(Guid mapId, IEnumerable<EntityId> entityIds, long? tickNumber = null)
    {
        var packet = new KeyframePacket
        {
            TickNumber = tickNumber ?? world.TickCount,
            MapId = mapId
        };

        foreach (var entityId in entityIds)
        {
            var state = world.Entities.Get(entityId);
            if (state == null) continue;

            var kind = DetermineKind(state);
            var snapshot = new KeyframeEntity
            {
                EntityId = entityId.Value,
                Kind = kind
            };

            foreach (var (type, obj) in state.GetAllComponents())
            {
                var tag = ComponentTypeRegistry.Tag(type);
                if (tag == null) continue;
                var data = MemoryPackSerializer.Serialize(type, obj);
                snapshot.Components.Add(new ComponentData { Tag = tag.Value, Data = data });
            }

            if (snapshot.Components.Count > 0)
                packet.Entities.Add(snapshot);
        }

        return packet;
    }

    private static EntityKind DetermineKind(EntityState state) =>
        state.Has<PlayerTag>() ? EntityKind.Player
        : state.Has<NpcTag>() ? EntityKind.Npc
        : state.Has<GroundItemTag>() ? EntityKind.GroundItem
        : EntityKind.Player;
}
