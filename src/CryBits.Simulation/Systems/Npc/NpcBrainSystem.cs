using CryBits.Definitions.Catalog;
using CryBits.Definitions.Utils;
using CryBits.Definitions.Common;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Npcs;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Events;
using CryBits.Simulation.State;
using CryBits.Simulation.Systems.Npc.Behaviors;
using static CryBits.Simulation.SimulationConstants;

namespace CryBits.Simulation.Systems.Npc;

public sealed class NpcBrainSystem(DefinitionCatalog catalog) : ISimulationSystem
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

        foreach (var map in world.Maps.Values)
        {
            if (!map.HasPlayers(world.Entities)) continue;

            foreach (var npcId in map.NpcIds)
            {
                var e = world.Entities.Get(npcId);
                if (e == null) continue;
                var npcState = e.Get<NpcState>();
                if (npcState == null) continue;

                UpdateTarget(world, npcId, tick);
                if (npcState.TargetId.HasValue)
                {
                    var targetE = world.Entities.Get(npcState.TargetId.Value);
                    FaceTarget(world, e, targetE?.Get<Position>());
                }

                var npcData = catalog.Npcs.Get(npcState.NpcDefId);
                if (npcData == null) continue;

                var behavior = PickBehavior(e, npcData, npcState);
                var intent = behavior.GetNextAction(world, e, npcData, tick);
                if (intent != null)
                    tick.Intents.Enqueue(intent);
            }
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

    private static void FaceTarget(World world, EntityState entity, Position? targetPos)
    {
        var pos = entity.Get<Position>()!;
        if (targetPos == null) return;

        Direction dir;
        if (Math.Abs(pos.X - targetPos.X) >= Math.Abs(pos.Y - targetPos.Y))
            dir = pos.X > targetPos.X ? Direction.Left : Direction.Right;
        else
            dir = pos.Y > targetPos.Y ? Direction.Up : Direction.Down;

        if (pos.Direction != dir)
        {
            pos.Direction = dir;
            world.MarkDirty<Position>(entity.Id);
        }
    }

    private void UpdateTarget(World world, EntityId npcId, Tick tick)
    {
        var e = world.Entities.Get(npcId);
        if (e == null) return;
        var npcState = e.Get<NpcState>()!;
        var pos = e.Get<Position>()!;
        var npcData = catalog.Npcs.Get(npcState.NpcDefId);
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
                        npcState.TargetId = null;
                }
                else if (targetE.Has<NpcTag>())
                {
                    var targetNpcState = targetE.Get<NpcState>();
                    var targetPos = targetE.Get<Position>()!;
                    if (targetNpcState == null || targetPos.MapId != pos.MapId)
                    {
                        npcState.TargetId = null;
                        world.MarkDirty<NpcState>(npcId);
                    }
                }
            }
            else
            {
                npcState.TargetId = null;
                world.MarkDirty<NpcState>(npcId);
            }
        }

        if (npcState.TargetId.HasValue)
        {
            var targetE = world.Entities.Get(npcState.TargetId.Value);
            if (targetE == null) return;
            var targetPos = targetE.Get<Position>()!;
            var distance = Math.Sqrt(Math.Pow(pos.X - targetPos.X, 2) + Math.Pow(pos.Y - targetPos.Y, 2));
            if (npcData.Sight < distance)
            {
                npcState.TargetId = null;
                world.MarkDirty<NpcState>(npcId);
            }
        }
    }

    private void ScanForTarget(World world, EntityId npcId, Tick tick)
    {
        var e = world.Entities.Get(npcId);
        if (e == null) return;
        var npcState = e.Get<NpcState>()!;
        var pos = e.Get<Position>()!;
        var npcData = catalog.Npcs.Get(npcState.NpcDefId);
        if (npcData is null) return;
        var map = world.Maps.Get(pos.MapId);
        if (map == null) return;

        short distance;

        foreach (var state in world.Entities.All)
        {
            if (!state.Has<PlayerTag>()) continue;
            var targetPos = state.Get<Position>();
            if (targetPos == null || targetPos.MapId != pos.MapId) continue;

            distance = (short)Math.Sqrt(Math.Pow(pos.X - targetPos.X, 2) +
                                        Math.Pow(pos.Y - targetPos.Y, 2));
            if (distance <= npcData.Sight)
            {
                npcState.TargetId = state.Id;
                world.MarkDirty<NpcState>(npcId);
                if (!string.IsNullOrEmpty(npcData.SayMsg))
                    tick.Events.Emit(new ChatMessageEvent
                    {
                        RecipientId = state.Id,
                        Text = npcData.Name + ": " + npcData.SayMsg,
                        ColorArgb = ChatColors.White
                    });
                return;
            }
        }

        if (!npcData.AttackNpc) return;

        foreach (var otherNpcId in map.NpcIds)
        {
            if (otherNpcId == npcId) continue;
            var otherE = world.Entities.Get(otherNpcId);
            if (otherE == null) continue;
            var otherNpcState = otherE.Get<NpcState>();
            if (otherNpcState == null) continue;
            var otherPos = otherE.Get<Position>();
            if (otherPos == null) continue;
            var otherData = catalog.Npcs.Get(otherNpcState.NpcDefId);
            if (otherData is null) continue;
            if (npcData.IsAllied(otherData.Id)) continue;

            distance = (short)Math.Sqrt(Math.Pow(pos.X - otherPos.X, 2) +
                                        Math.Pow(pos.Y - otherPos.Y, 2));
            if (distance <= npcData.Sight)
            {
                npcState.TargetId = otherNpcId;
                world.MarkDirty<NpcState>(npcId);
                return;
            }
        }
    }
}
