using CryBits.Definitions.Catalog;
using CryBits.Simulation.Core;
using CryBits.Simulation.Systems.Combat;
using CryBits.Simulation.Systems.Inventory;
using CryBits.Simulation.Systems.Movement;
using CryBits.Simulation.Systems.Npc;
using CryBits.Simulation.Systems.Party;
using CryBits.Simulation.Systems.Progression;
using CryBits.Simulation.Systems.Regeneration;
using CryBits.Simulation.Systems.Shops;
using CryBits.Simulation.Systems.Spawning;
using CryBits.Simulation.Systems.Trade;

namespace CryBits.Host;

internal static class HostPipelineBuilder
{
    public static TickPipeline Build(DefinitionCatalog catalog)
    {
        var pipeline = new TickPipeline();
        pipeline.AddSystem(new VitalsRegenSystem());
        pipeline.AddSystem(new NpcBrainSystem(catalog));
        pipeline.AddSystem(new MovementSystem());
        pipeline.AddSystem(new CombatSystem(catalog));
        pipeline.AddSystem(new AggroSystem());
        pipeline.AddSystem(new LevelingSystem(catalog));
        pipeline.AddSystem(new DeathSystem(catalog));
        pipeline.AddSystem(new GroundItemSystem(catalog));
        pipeline.AddSystem(new EquipmentSystem(catalog));
        pipeline.AddSystem(new InventorySystem(catalog));
        pipeline.AddSystem(new HotbarSystem());
        pipeline.AddSystem(new TradeSystem());
        pipeline.AddSystem(new ShopSystem(catalog));
        pipeline.AddSystem(new PartySystem());
        pipeline.AddSystem(new NpcRespawnSystem(catalog));
        return pipeline;
    }
}
