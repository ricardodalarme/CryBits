using CryBits.Definitions.Common;
using CryBits.Definitions.Npcs;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Intents;

namespace CryBits.Simulation.Systems.Npc.Behaviors;

public sealed class IdleBehavior : INpcBehavior
{
    public Intent? GetNextAction(World world, EntityId entity, Definitions.Npcs.Npc npcData)
    {
        if (npcData.Movement != MovementStyle.TurnRandomly) return null;
        if (Random.Shared.Next(0, 3) != 0) return null;

        var dir = (Direction)Random.Shared.Next(0, 4);
        world.Update<Position>(entity, p => p with { Direction = dir });
        return null;
    }
}
