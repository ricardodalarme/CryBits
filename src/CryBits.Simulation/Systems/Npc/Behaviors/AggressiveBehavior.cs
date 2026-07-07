using CryBits.Definitions.Common;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Intents;
using CryBits.Simulation.Spatial;
using CryBits.Simulation.State;

namespace CryBits.Simulation.Systems.Npc.Behaviors;

public sealed class AggressiveBehavior : INpcBehavior
{
    public Intent? GetNextAction(World world, EntityState entity, Definitions.Npcs.Npc npcData, Tick tick)
    {
        var npcState = entity.Get<NpcState>()!;
        if (!npcState.TargetId.HasValue) return null;

        var targetE = world.Entities.Get(npcState.TargetId.Value);
        if (targetE == null) return null;

        var pos = entity.Get<Position>()!;
        var targetPos = targetE.Get<Position>();
        if (targetPos == null) return null;

        var dx = pos.X > targetPos.X ? pos.X - targetPos.X : targetPos.X - pos.X;
        var dy = pos.Y > targetPos.Y ? pos.Y - targetPos.Y : targetPos.Y - pos.Y;

        if (dx + dy <= 1)
        {
            FaceTarget(world, entity, targetPos);
            return new AttackIntent(entity.Id, npcState.TargetId.Value);
        }

        if (entity.Has<PathFollow>())
            return null;

        var path = Pathfinder.FindPath(world, pos.MapId, pos.X, pos.Y, targetPos.X, targetPos.Y);
        if (path != null)
        {
            entity.Set(new PathFollow(path));
            return null;
        }

        return null;
    }

    private static void FaceTarget(World world, EntityState entity, Position targetPos)
    {
        var pos = entity.Get<Position>()!;

        Direction dir;
        if (Math.Abs(pos.X - targetPos.X) >= Math.Abs(pos.Y - targetPos.Y))
            dir = pos.X > targetPos.X ? Direction.Left : Direction.Right;
        else
            dir = pos.Y > targetPos.Y ? Direction.Up : Direction.Down;

        if (pos.Direction != dir)
            world.Update<Position>(entity.Id, p => p with { Direction = dir });
    }
}
