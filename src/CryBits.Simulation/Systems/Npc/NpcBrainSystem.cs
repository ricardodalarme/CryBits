using CryBits.Definitions.Common;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Npcs;
using CryBits.Definitions.Utils;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Events;
using CryBits.Simulation.Spatial;
using CryBits.Simulation.State;
using CryBits.Simulation.Systems.Npc.Behaviors;
using static CryBits.Simulation.SimulationConstants;

namespace CryBits.Simulation.Systems.Npc;

public sealed class NpcBrainSystem : ISimulationSystem
{
    private readonly INpcBehavior _idle = new IdleBehavior();
    private readonly INpcBehavior _wander = new WanderBehavior();
    private readonly INpcBehavior _aggressive = new AggressiveBehavior();
    private readonly INpcBehavior _flee = new FleeBehavior();
    private long _lastTick;

    public void Execute(World world, Tick tick)
    {
        if (tick.TickNumber - _lastTick < TicksPerSecond / 2) return;
        _lastTick = tick.TickNumber;

        foreach (var e in world.Entities.All)
        {
            if (!e.Has<NpcTag>()) continue;
            var npcState = e.Get<NpcState>();
            if (npcState == null) continue;

            var pathFollow = e.Get<PathFollow>();
            if (pathFollow != null && !pathFollow.IsComplete)
            {
                if (npcState.TargetId.HasValue && world.IsAlive(npcState.TargetId.Value))
                    continue;
                e.Remove<PathFollow>();
            }

            UpdateTarget(world, e.Id, tick);

            var npcData = world.Catalog.Npcs.Get(npcState.NpcDefId);
            if (npcData == null) continue;

            var behavior = PickBehavior(e, npcData, npcState);
            var intent = behavior.GetNextAction(world, e, npcData, tick);
            if (intent != null)
                tick.Intents.Enqueue(intent);
        }
    }

    private INpcBehavior PickBehavior(EntityState entity, Definitions.Npcs.Npc npcData, NpcState npcState)
    {
        if (!npcState.TargetId.HasValue)
            return npcData.Movement == MovementStyle.MoveRandomly ? _wander : _idle;

        var vitals = entity.Get<Vitals>()!;
        if (vitals.Hp * 100 <= vitals.MaxHp * npcData.FleeHealth)
            return _flee;

        return _aggressive;
    }

    private void UpdateTarget(World world, EntityId npcId, Tick tick)
    {
        var e = world.Entities.Get(npcId);
        if (e == null) return;
        var npcState = e.Get<NpcState>()!;
        var pos = e.Get<Position>()!;
        var npcData = world.Catalog.Npcs.Get(npcState.NpcDefId);
        if (npcData is null) return;

        if (npcData.Behaviour == Behaviour.AttackOnSight && !npcState.TargetId.HasValue)
            ScanForTarget(world, npcId, tick);

        if (npcState.TargetId.HasValue)
        {
            var targetE = world.Entities.Get(npcState.TargetId.Value);
            if (targetE != null)
            {
                if (targetE.Has<PlayerTag>())
                {
                    var targetPos = targetE.Get<Position>()!;
                    if (targetPos.MapId != pos.MapId)
                        world.Update<NpcState>(npcId, s => s with { TargetId = null });
                }
                else if (targetE.Has<NpcTag>())
                {
                    var targetNpcState = targetE.Get<NpcState>();
                    var targetPos = targetE.Get<Position>()!;
                    if (targetNpcState == null || targetPos.MapId != pos.MapId)
                        world.Update<NpcState>(npcId, s => s with { TargetId = null });
                }
            }
            else
                world.Update<NpcState>(npcId, s => s with { TargetId = null });
        }

        if (npcState.TargetId.HasValue)
        {
            var targetE = world.Entities.Get(npcState.TargetId.Value);
            if (targetE == null) return;
            var targetPos = targetE.Get<Position>()!;
            var distance = Math.Sqrt(Math.Pow(pos.X - targetPos.X, 2) + Math.Pow(pos.Y - targetPos.Y, 2));
            if (npcData.Sight < distance)
                world.Update<NpcState>(npcId, s => s with { TargetId = null });
        }
    }

    private void ScanForTarget(World world, EntityId npcId, Tick tick)
    {
        var e = world.Entities.Get(npcId);
        if (e == null) return;
        var npcState = e.Get<NpcState>()!;
        var pos = e.Get<Position>()!;
        var npcData = world.Catalog.Npcs.Get(npcState.NpcDefId);
        if (npcData is null) return;

        var npcChunk = ChunkGrid.FromPosition(pos.X, pos.Y);
        var nearby = world.SpatialGrid.GetNeighborhood(npcChunk, 2);

        foreach (var id in world.SpatialGrid.GetEntities(nearby))
        {
            if (id == npcId) continue;
            var targetPos = world.Get<Position>(id);
            if (targetPos == null || targetPos.MapId != pos.MapId) continue;

            var dx = pos.X - targetPos.X;
            var dy = pos.Y - targetPos.Y;
            var distSq = dx * dx + dy * dy;

            if (distSq > npcData.Sight * npcData.Sight) continue;

            if (world.Has<PlayerTag>(id))
            {
                world.Update<NpcState>(npcId, s => s with { TargetId = id });
                if (!string.IsNullOrEmpty(npcData.SayMsg))
                    tick.Events.Emit(new ChatMessageEvent(tick.TickNumber, id, npcData.Name + ": " + npcData.SayMsg, ChatColors.White));
                return;
            }

            if (npcData.AttackNpc && world.Has<NpcTag>(id))
            {
                var otherNpcState = world.Get<NpcState>(id);
                if (otherNpcState == null) continue;
                var otherData = world.Catalog.Npcs.Get(otherNpcState.NpcDefId);
                if (otherData is null || npcData.IsAlly(otherData.Id)) continue;
                world.Update<NpcState>(npcId, s => s with { TargetId = id });
                return;
            }
        }
    }
}
