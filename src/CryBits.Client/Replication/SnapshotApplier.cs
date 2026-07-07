using CryBits.Client.Components;
using CryBits.Client.Core;
using CryBits.Client.Spawners;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Protocol.Packets;
using CryBits.Protocol.Packets.Server;
using CryBits.Protocol.Serialization;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using MemoryPack;
using EntityKind = CryBits.Protocol.Packets.Server.EntityKind;

namespace CryBits.Client.Replication;

internal sealed class SnapshotApplier(
    World world,
    GameContext context,
    DefinitionCatalog catalog)
{
    private long _lastAppliedTick;

    public long LastAppliedTick => _lastAppliedTick;

    public void Apply(KeyframePacket packet)
    {
        var receivedNetworkIds = new HashSet<long>();

        foreach (var entity in packet.Entities)
        {
            var serverId = entity.EntityId;
            receivedNetworkIds.Add(serverId);

            var localId = context.GetNetworkEntity(serverId);

            if (localId == null)
            {
                SpawnEntity(serverId, entity.Kind, entity.Components);
                localId = context.GetNetworkEntity(serverId);
            }

            if (localId != null)
                ApplyComponents(localId.Value, entity.Components);
        }

        _lastAppliedTick = Math.Max(_lastAppliedTick, packet.TickNumber);
        context.LastAppliedServerTick = _lastAppliedTick;
        PruneStaleEntities(packet.MapId, receivedNetworkIds);
    }

    public void Apply(DeltaPacket packet)
    {
        if (packet.BaselineTick > _lastAppliedTick)
        {
            context.RequestKeyframe();
            return;
        }

        _lastAppliedTick = Math.Max(_lastAppliedTick, packet.TickNumber);
        context.LastAppliedServerTick = _lastAppliedTick;

        foreach (var delta in packet.Entities)
        {
            var serverId = delta.EntityId;

            var localId = context.GetNetworkEntity(serverId);
            if (localId == null && delta.Action == DeltaAction.Added)
            {
                SpawnEntity(serverId, delta.Kind, delta.Components);
                localId = context.GetNetworkEntity(serverId);
            }

            if (localId != null)
            {
                ApplyComponents(localId.Value, delta.Components);
                foreach (var removedTag in delta.RemovedTags)
                {
                    var removedType = ComponentTypeRegistry.Type(removedTag);
                    if (removedType != null)
                        world.Remove(localId.Value, removedType);
                }
            }
        }

        foreach (var removedId in packet.RemovedEntities)
        {
            var localId = context.GetNetworkEntity(removedId);
            if (localId != null)
            {
                context.UnregisterNetworkEntity(removedId);
                world.Destroy(localId.Value);
            }
        }
    }

    private void ApplyComponents(EntityId localId, List<ComponentData> components)
    {
        if (!world.IsAlive(localId)) return;

        foreach (var comp in components)
        {
            var type = ComponentTypeRegistry.Type(comp.Tag);
            if (type == null) continue;
            var obj = MemoryPackSerializer.Deserialize(type, comp.Data);
            if (obj != null) world.Set(localId, obj);
        }
    }

    private void SpawnEntity(long serverId, EntityKind kind, List<ComponentData> components)
    {
        switch (kind)
        {
            case EntityKind.Player: SpawnPlayer(serverId, components); break;
            case EntityKind.Npc: SpawnNpc(serverId, components); break;
            case EntityKind.GroundItem: SpawnGroundItem(serverId, components); break;
        }
    }

    private void SpawnPlayer(long serverId, List<ComponentData> components)
    {
        var appearance = DeserializeComp<PlayerAppearance>(components);
        var position = DeserializeComp<Position>(components);
        var vitals = DeserializeComp<Vitals>(components);
        var stat = DeserializeComp<LevelComponent>(components);
        var attrs = DeserializeComp<AttributesComponent>(components);
        var equip = DeserializeComp<EquipmentState>(components);

        if (appearance == null || position == null) return;

        var isLocal = serverId == context.LocalPlayerId;

        var vitalArray = vitals != null
            ? new short[] { vitals.Hp, vitals.Mp }
            : [0, 0];
        var maxVitalArray = vitals != null
            ? new short[] { vitals.MaxHp, vitals.MaxMp }
            : [0, 0];
        var equipItems = equip != null
            ? Array.ConvertAll(equip.Slots, id => catalog.Items.Get(id))
            : [];

        EntityId localEntity;
        if (isLocal)
            localEntity = PlayerSpawner.SpawnLocal(world, serverId,
                appearance.Name, appearance.TextureNum,
                stat?.Level ?? 1, vitalArray, maxVitalArray,
                attrs?.Values ?? [],
                equipItems, position.X, position.Y,
                position.Direction);
        else
            localEntity = PlayerSpawner.Spawn(world, serverId,
                appearance.Name, appearance.TextureNum,
                vitalArray, maxVitalArray, position.X, position.Y,
                position.Direction);

        context.RegisterNetworkEntity(serverId, localEntity);

        if (isLocal)
        {
            var inv = DeserializeComp<InventoryState>(components);
            var hotbar = DeserializeComp<HotbarState>(components);
            if (stat != null) world.Set(localEntity, stat);
            if (inv != null) world.Set(localEntity, inv);
            if (equip != null) world.Set(localEntity, equip);
            if (hotbar != null) world.Set(localEntity, hotbar);
            if (attrs != null) world.Set(localEntity, attrs);
        }
    }

    private void SpawnNpc(long serverId, List<ComponentData> components)
    {
        var npcState = DeserializeComp<NpcState>(components);
        var position = DeserializeComp<Position>(components);
        var vitals = DeserializeComp<Vitals>(components);
        if (npcState == null || position == null) return;

        var npcDef = catalog.Npcs.Get(npcState.NpcDefId);
        if (npcDef == null) return;

        var localEntity = NpcSpawner.Spawn(world, serverId, npcDef,
            position.X, position.Y, position.Direction, vitals ?? new Vitals(0, 0, 0, 0));
        context.RegisterNetworkEntity(serverId, localEntity);
    }

    private void SpawnGroundItem(long serverId, List<ComponentData> components)
    {
        var groundItem = DeserializeComp<GroundItem>(components);
        var position = DeserializeComp<Position>(components);
        if (groundItem == null || position == null) return;

        var item = catalog.Items.Get(groundItem.ItemDefId);
        if (item == null) return;

        var localEntity = GroundItemSpawner.Spawn(world, serverId, item, position);
        context.RegisterNetworkEntity(serverId, localEntity);
    }

    private void PruneStaleEntities(Guid mapId, HashSet<long> receivedNetworkIds)
    {
        var toDestroy = new List<EntityId>();
        foreach (var entityId in world.All)
        {
            var pos = world.Get<Position>(entityId);
            var nid = world.Get<NetworkId>(entityId);
            if (pos != null && pos.MapId == mapId && nid != null
                && !receivedNetworkIds.Contains(nid.Value))
            {
                toDestroy.Add(entityId);
            }
        }
        foreach (var id in toDestroy)
        {
            var nid = world.Get<NetworkId>(id);
            if (nid != null) context.UnregisterNetworkEntity(nid.Value);
            world.Destroy(id);
        }
    }

    private static T? DeserializeComp<T>(List<ComponentData> components) where T : class
    {
        var tag = ComponentTypeRegistry.Tag(typeof(T));
        if (tag == null) return null;
        var comp = components.FirstOrDefault(c => c.Tag == tag.Value);
        if (comp == null) return null;
        return MemoryPackSerializer.Deserialize<T>(comp.Data);
    }
}
