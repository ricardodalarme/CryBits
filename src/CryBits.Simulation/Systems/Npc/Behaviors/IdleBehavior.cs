using CryBits.Definitions.Common;
using CryBits.Definitions.Npcs;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Intents;
using CryBits.Simulation.State;

namespace CryBits.Simulation.Systems.Npc.Behaviors;

public sealed class IdleBehavior : INpcBehavior
{
    public Intent? GetNextAction(World world, EntityState entity, Definitions.Npcs.Npc npcData, Tick tick)
    {
        if (npcData.Movement != MovementStyle.TurnRandomly) return null;
        if (Random.Shared.Next(0, 3) != 0) return null;

        var pos = entity.Get<Position>()!;
        pos.Direction = (Direction)Random.Shared.Next(0, 4);
        world.MarkDirty<Position>(entity.Id);
        return null;
    }
}
