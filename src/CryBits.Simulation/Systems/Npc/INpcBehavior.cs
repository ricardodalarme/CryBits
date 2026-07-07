using CryBits.Simulation.Core;
using CryBits.Simulation.Intents;

namespace CryBits.Simulation.Systems.Npc;

public interface INpcBehavior
{
    Intent? GetNextAction(World world, EntityId entity, Definitions.Npcs.Npc npcData, Tick tick);
}
