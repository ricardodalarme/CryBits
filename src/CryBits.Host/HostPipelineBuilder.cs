using CryBits.Host.Replication;
using CryBits.Simulation.Core;
using CryBits.Simulation.Systems.Chat;
using CryBits.Simulation.Systems.Combat;
using CryBits.Simulation.Systems.Inventory;
using CryBits.Simulation.Systems.Movement;
using CryBits.Simulation.Systems.Npc;
using CryBits.Simulation.Systems.Progression;
using CryBits.Simulation.Systems.Regeneration;
using CryBits.Simulation.Systems.Shops;
using CryBits.Simulation.Systems.Spawning;

namespace CryBits.Host;

internal static class HostPipelineBuilder
{
    public static TickPipeline Build(DeltaReplicator deltaReplicator)
    {
        var pipeline = new TickPipeline();
        pipeline.AddSystem(new VitalsRegenSystem());
        pipeline.AddSystem(new NpcBrainSystem());
        pipeline.AddSystem(new PathFollowSystem());
        pipeline.AddSystem(new MovementSystem());
        pipeline.AddSystem(new CombatSystem());
        pipeline.AddSystem(new AggroSystem());
        pipeline.AddSystem(new LevelingSystem());
        pipeline.AddSystem(new DeathSystem());
        pipeline.AddSystem(new EquipmentSystem());
        pipeline.AddSystem(new InventorySystem());
        pipeline.AddSystem(new GroundItemSystem());
        pipeline.AddSystem(new HotbarSystem());
        pipeline.AddSystem(new ShopSystem());
        pipeline.AddSystem(new ChatSystem());
        pipeline.AddSystem(new NpcRespawnSystem());
        pipeline.AddSystem(deltaReplicator);
        return pipeline;
    }
}
