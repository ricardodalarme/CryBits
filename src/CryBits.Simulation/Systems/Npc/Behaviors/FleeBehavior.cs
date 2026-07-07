using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Intents;
using CryBits.Simulation.Spatial;

namespace CryBits.Simulation.Systems.Npc.Behaviors;

public sealed class FleeBehavior : INpcBehavior
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

        if (world.Has<PathFollow>(entity))
            return null;

        var fleeX = pos.X + (pos.X - targetPos.X) * 4;
        var fleeY = pos.Y + (pos.Y - targetPos.Y) * 4;

        var path = Pathfinder.FindPath(world, pos.MapId, pos.X, pos.Y, fleeX, fleeY, maxRange: 12);
        if (path != null)
        {
            world.Set(entity, new PathFollow(path));
            return null;
        }

        return null;
    }
}
