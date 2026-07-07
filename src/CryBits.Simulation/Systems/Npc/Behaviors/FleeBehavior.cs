using CryBits.Definitions.Common;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Intents;
using CryBits.Simulation.Spatial;
using CryBits.Simulation.State;

namespace CryBits.Simulation.Systems.Npc.Behaviors;

public sealed class FleeBehavior : INpcBehavior
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

        if (entity.Has<PathFollow>())
            return null;

        var fleeX = pos.X + (pos.X - targetPos.X) * 4;
        var fleeY = pos.Y + (pos.Y - targetPos.Y) * 4;

        var path = Pathfinder.FindPath(world, pos.MapId, pos.X, pos.Y, fleeX, fleeY, maxRange: 12);
        if (path != null)
        {
            entity.Set(new PathFollow(path));
            return null;
        }

        return null;
    }
}
