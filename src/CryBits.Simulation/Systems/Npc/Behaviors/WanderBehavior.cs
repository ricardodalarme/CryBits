using CryBits.Definitions.Common;
using CryBits.Simulation.Core;
using CryBits.Simulation.Intents;
using CryBits.Simulation.State;

namespace CryBits.Simulation.Systems.Npc.Behaviors;

public sealed class WanderBehavior : INpcBehavior
{
    public Intent? GetNextAction(World world, EntityState entity, Definitions.Npcs.Npc npcData, Tick tick)
    {
        if (Random.Shared.Next(0, 3) != 0) return null;

        var dir = (Direction)Random.Shared.Next(0, 4);
        return new MoveIntent(entity.Id, dir, Definitions.Common.Movement.Walking);
    }
}
