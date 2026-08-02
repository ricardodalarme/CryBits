using CryBits.Definitions.Common;
using CryBits.Simulation.Core;
using CryBits.Simulation.Intents;

namespace CryBits.Simulation.Systems.Npc.Behaviors;

public sealed class WanderBehavior : INpcBehavior
{
    public Intent? GetNextAction(World world, EntityId entity, Definitions.Npcs.Npc npcData)
    {
        if (Random.Shared.Next(0, 3) != 0) return null;

        var dir = (Direction)Random.Shared.Next(0, 4);
        return new MoveIntent(entity, dir, Definitions.Common.Movement.Walking);
    }
}
