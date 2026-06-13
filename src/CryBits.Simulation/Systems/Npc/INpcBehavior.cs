using CryBits.Simulation.Core;
using CryBits.Simulation.Intents;
using CryBits.Simulation.State;

namespace CryBits.Simulation.Systems.Npc;

public interface INpcBehavior
{
    Intent? GetNextAction(World world, EntityState entity, Definitions.Npcs.Npc npcData, Tick tick);
}
