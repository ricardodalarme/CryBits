using CryBits.Protocol.Packets;
using CryBits.Protocol.Packets.Server;
using CryBits.Protocol.Serialization;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.State;
using MemoryPack;

namespace CryBits.Host.Replication;

public sealed class KeyframeEncoder(World world)
{
    public KeyframePacket Encode(Guid mapId, IEnumerable<EntityId> entityIds)
    {
        var packet = new KeyframePacket
        {
            TickNumber = world.TickCount,
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
