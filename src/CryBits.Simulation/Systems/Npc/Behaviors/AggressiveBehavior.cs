using CryBits.Definitions.Common;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Intents;
using CryBits.Simulation.Spatial;

namespace CryBits.Simulation.Systems.Npc.Behaviors;

public sealed class AggressiveBehavior : INpcBehavior
{
    public Intent? GetNextAction(World world, EntityId entity, Definitions.Npcs.Npc npcData, Tick tick)
    {
        var npcState = world.Get<NpcState>(entity)!;
        if (!npcState.TargetId.HasValue) return null;

        var targetE = npcState.TargetId.Value;
        if (!world.IsAlive(targetE)) return null;

        var pos = world.Get<Position>(entity)!;
        var targetPos = world.Get<Position>(targetE);
        if (targetPos == null) return null;

        var dx = pos.X > targetPos.X ? pos.X - targetPos.X : targetPos.X - pos.X;
        var dy = pos.Y > targetPos.Y ? pos.Y - targetPos.Y : targetPos.Y - pos.Y;

        if (dx + dy <= 1)
        {
            FaceTarget(world, entity, targetPos);
            return new AttackIntent(entity, npcState.TargetId.Value);
        }

        if (world.Has<PathFollow>(entity))
            return null;

        var path = Pathfinder.FindPath(world, pos.MapId, pos.X, pos.Y, targetPos.X, targetPos.Y);
        if (path != null)
        {
            world.Set(entity, new PathFollow(path));
            return null;
        }

        return null;
    }

    private static void FaceTarget(World world, EntityId entity, Position targetPos)
    {
        var pos = world.Get<Position>(entity)!;

        Direction dir;
        if (Math.Abs(pos.X - targetPos.X) >= Math.Abs(pos.Y - targetPos.Y))
            dir = pos.X > targetPos.X ? Direction.Left : Direction.Right;
        else
            dir = pos.Y > targetPos.Y ? Direction.Up : Direction.Down;

        if (pos.Direction != dir)
            world.Update<Position>(entity, p => p with { Direction = dir });
    }
}
