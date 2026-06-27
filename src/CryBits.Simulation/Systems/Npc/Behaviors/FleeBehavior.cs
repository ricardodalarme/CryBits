using CryBits.Definitions.Common;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Intents;
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

        Direction dir;
        if (Math.Abs(pos.X - targetPos.X) >= Math.Abs(pos.Y - targetPos.Y))
            dir = pos.X > targetPos.X ? Direction.Right : Direction.Left;
        else
            dir = pos.Y > targetPos.Y ? Direction.Down : Direction.Up;

        return new MoveIntent(entity.Id, dir, Definitions.Common.Movement.Walking);
    }
}
