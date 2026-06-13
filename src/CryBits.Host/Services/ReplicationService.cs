using CryBits.Host.Network.Senders;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Events;
using CryBits.Simulation.State;

namespace CryBits.Host.Services;

internal sealed class ReplicationService(
    PlayerSender playerSender,
    NpcSender npcSender,
    MapSender mapSender,
    CombatSender combatSender) : ISimulationSystem
{
    public void Execute(World world, Tick tick)
    {
        // 1. Replicate component changes via dirty tracking
        foreach (var (entityId, componentType) in world.Dirty.All)
        {
            var entity = world.Entities.Get(entityId);
            if (entity == null) continue;

            if (componentType == typeof(Position))
                ReplicatePosition(world, entityId, entity.Get<Position>()!);
            else if (componentType == typeof(Vitals))
                ReplicateVitals(world, entityId, entity);
            else if (componentType == typeof(StatBlock))
                ReplicateStats(world, entityId, entity);
            else if (componentType == typeof(InventoryState))
                playerSender.PlayerInventory(entityId);
            else if (componentType == typeof(EquipmentState))
                playerSender.PlayerEquipments(entityId);
            else if (componentType == typeof(HotbarState))
                playerSender.PlayerHotbar(entityId);
            else if (componentType == typeof(NpcState))
                ReplicateNpcState(world, entityId, entity);
        }

        // 2. Replicate tick events
        foreach (var ev in tick.Events.Events)
        {
            if (ev is CombatAttackEvent attack)
                combatSender.Attack(attack.MapId, attack.AttackerId, attack.VictimId);

            if (ev is MapGroundItemsChangedEvent items)
                ReplicateMapItemsChanged(world, items);

            if (ev is PlayerWarpedEvent warp && warp.NeedsMapData)
                ReplicatePlayerWarp(world, warp);
        }

        world.Dirty.Clear();
    }

    private void ReplicatePosition(World world, EntityId entityId, Position pos)
    {
        var entity = world.Entities.Get(entityId);
        if (entity == null) return;
        var combat = entity.Get<CombatState>();

        if (combat?.GettingMap == true) return;

        if (entity.Has<PlayerTag>())
            playerSender.PlayerMove(entityId, 1);
        else if (entity.Has<NpcTag>())
            npcSender.MapNpcMovement(entityId, 1);
    }

    private void ReplicateVitals(World world, EntityId entityId, EntityState entity)
    {
        if (entity.Has<PlayerTag>())
            playerSender.PlayerVitals(entityId);
        else if (entity.Has<NpcTag>())
            npcSender.MapNpcVitals(entityId);
    }

    private void ReplicateStats(World world, EntityId entityId, EntityState entity)
    {
        if (!entity.Has<PlayerTag>()) return;
        playerSender.PlayerExperience(entityId);
        var pos = entity.Get<Position>();
        if (pos != null)
            mapSender.MapPlayers(entityId);
    }

    private void ReplicateNpcState(World world, EntityId entityId, EntityState entity)
    {
        var npcState = entity.Get<NpcState>();
        if (npcState == null) return;

        if (npcState.Alive)
            npcSender.MapNpc(entityId);
        else
            npcSender.MapNpcDied(entityId);
    }

    private void ReplicateMapItemsChanged(World world, MapGroundItemsChangedEvent items)
    {
        foreach (var map in world.Maps.Values)
        {
            if (map.Id == items.MapId)
            {
                mapSender.MapItems(map);
                break;
            }
        }
    }

    private void ReplicatePlayerWarp(World world, PlayerWarpedEvent warp)
    {
        var entityId = new EntityId(warp.PlayerId);
        foreach (var map in world.Maps.Values)
        {
            if (map.Id == warp.NewMapId)
            {
                mapSender.MapRevision(entityId, map.Data);
                mapSender.MapItems(entityId, map);
                npcSender.MapNpcs(entityId, map);
                break;
            }
        }
    }
}
