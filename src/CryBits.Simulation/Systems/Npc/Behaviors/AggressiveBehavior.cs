using CryBits.Definitions.Common;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Intents;
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

        return dx <= 1 && dy <= 1
            ? new AttackIntent(entity.Id, npcState.TargetId.Value)
            : MoveToward(pos, targetPos, entity.Id);
    }

    private static Intent? MoveToward(Position pos, Position target, EntityId entityId)
    {
        var dx = pos.X > target.X ? pos.X - target.X : target.X - pos.X;
        var dy = pos.Y > target.Y ? pos.Y - target.Y : target.Y - pos.Y;

        if (dx >= dy)
        {
            if (pos.X != target.X)
                return new MoveIntent(entityId, pos.X > target.X ? Direction.Left : Direction.Right, CryBits.Definitions.Common.Movement.Walking);
            if (pos.Y != target.Y)
                return new MoveIntent(entityId, pos.Y > target.Y ? Direction.Up : Direction.Down, CryBits.Definitions.Common.Movement.Walking);
        }
        else
        {
            if (pos.Y != target.Y)
                return new MoveIntent(entityId, pos.Y > target.Y ? Direction.Up : Direction.Down, CryBits.Definitions.Common.Movement.Walking);
            if (pos.X != target.X)
                return new MoveIntent(entityId, pos.X > target.X ? Direction.Left : Direction.Right, CryBits.Definitions.Common.Movement.Walking);
        }

        return null;
    }
}
