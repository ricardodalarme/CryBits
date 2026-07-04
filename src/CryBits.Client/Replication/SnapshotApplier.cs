using CryBits.Client.Components;
using CryBits.Client.Core;
using CryBits.Client.Spawners;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Protocol.Serialization;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.State;
using MemoryPack;
using EntityKind = CryBits.Protocol.Packets.Server.EntityKind;
using ProtocolEntity = CryBits.Protocol.Packets.Server.KeyframeEntity;

namespace CryBits.Client.Replication;

internal sealed class SnapshotApplier(
    World world,
    GameContext context,
    DefinitionCatalog catalog)
{
    public void Apply(Protocol.Packets.Server.KeyframePacket packet)
    {
        var receivedNetworkIds = new HashSet<long>();

        foreach (var entity in packet.Entities)
        {
            var serverId = entity.EntityId;
            receivedNetworkIds.Add(serverId);

            var localId = context.GetNetworkEntity(serverId);

            if (localId == null)
            {
                SpawnEntity(entity, serverId);
            }
            else
            {
                var state = world.Entities.Get(localId.Value);
                if (state == null) continue;
                foreach (var comp in entity.Components)
                {
                    var type = ComponentTypeRegistry.Type(comp.Tag);
                    if (type == null) continue;
                    var obj = MemoryPackSerializer.Deserialize(type, comp.Data);
                    if (obj != null) state.Set(type, obj);
                }
            }
        }

        PruneStaleEntities(packet.MapId, receivedNetworkIds);
    }

    private void SpawnEntity(ProtocolEntity entity, long serverId)
    {
        switch (entity.Kind)
        {
            case EntityKind.Player: SpawnPlayer(entity, serverId); break;
            case EntityKind.Npc: SpawnNpc(entity, serverId); break;
            case EntityKind.GroundItem: SpawnGroundItem(entity, serverId); break;
        }
    }

    private void SpawnPlayer(ProtocolEntity entity, long serverId)
    {
        var appearance = DeserializeComp<PlayerAppearance>(entity);
        var position = DeserializeComp<Position>(entity);
        var vitals = DeserializeComp<Vitals>(entity);
        var stat = DeserializeComp<LevelComponent>(entity);
        var attrs = DeserializeComp<AttributesComponent>(entity);
        var equip = DeserializeComp<EquipmentState>(entity);

        if (appearance == null || position == null) return;

        var isLocal = serverId == context.LocalPlayer.Id;

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
            context.LocalPlayer.Entity = localEntity;
            var inv = DeserializeComp<InventoryState>(entity);
            var hotbar = DeserializeComp<HotbarState>(entity);
            if (stat != null) world.Set(localEntity, stat);
            if (inv != null) world.Set(localEntity, inv);
            if (equip != null) world.Set(localEntity, equip);
            if (hotbar != null) world.Set(localEntity, hotbar);
            if (attrs != null) world.Set(localEntity, attrs);
        }
    }

    private void SpawnNpc(ProtocolEntity entity, long serverId)
    {
        var npcState = DeserializeComp<NpcState>(entity);
        var position = DeserializeComp<Position>(entity);
        var vitals = DeserializeComp<Vitals>(entity);
        if (npcState == null || position == null) return;

        var npcDef = catalog.Npcs.Get(npcState.NpcDefId);
        if (npcDef == null) return;

        var localEntity = NpcSpawner.Spawn(world, serverId, npcDef,
            position.X, position.Y, position.Direction, vitals ?? new Vitals(0, 0, 0, 0));
        context.RegisterNetworkEntity(serverId, localEntity);
    }

    private void SpawnGroundItem(ProtocolEntity entity, long serverId)
    {
        var groundItem = DeserializeComp<GroundItem>(entity);
        var position = DeserializeComp<Position>(entity);
        if (groundItem == null || position == null) return;

        var item = catalog.Items.Get(groundItem.ItemDefId);
        if (item == null) return;

        var localEntity = GroundItemSpawner.Spawn(world, serverId, item, position);
        context.RegisterNetworkEntity(serverId, localEntity);
    }

    private void PruneStaleEntities(Guid mapId, HashSet<long> receivedNetworkIds)
    {
        var toDestroy = new List<EntityId>();
        foreach (var state in world.All)
        {
            var pos = state.Get<Position>();
            var nid = state.Get<NetworkId>();
            if (pos != null && pos.MapId == mapId && nid != null
                && !receivedNetworkIds.Contains(nid.Value))
            {
                toDestroy.Add(state.Id);
            }
        }
        foreach (var id in toDestroy)
        {
            var nid = world.Get<NetworkId>(id);
            if (nid != null) context.UnregisterNetworkEntity(nid.Value);
            world.Destroy(id);
        }
    }

    private static T? DeserializeComp<T>(ProtocolEntity entity) where T : class
    {
        var tag = ComponentTypeRegistry.Tag(typeof(T));
        if (tag == null) return null;
        var comp = entity.Components.FirstOrDefault(c => c.Tag == tag.Value);
        if (comp == null) return null;
        return MemoryPackSerializer.Deserialize<T>(comp.Data);
    }
}
