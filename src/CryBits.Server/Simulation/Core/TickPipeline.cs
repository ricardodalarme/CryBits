using CryBits.Server.Network.Senders;
using CryBits.Server.Systems.Combat;
using CryBits.Server.Systems.Inventory;
using CryBits.Server.Systems.Movement;
using CryBits.Server.Systems.Npc;
using CryBits.Server.Systems.Party;
using CryBits.Server.Systems.Progression;
using CryBits.Server.Systems.Shops;
using CryBits.Server.Systems.Spawning;
using CryBits.Server.Systems.Trade;
using CryBits.Simulation.Core;
using System.Collections.Generic;

namespace CryBits.Server.Simulation.Core;

internal sealed class TickPipeline
{
    private readonly List<ISimulationSystem> _systems = [];

    public void Add<T>() where T : ISimulationSystem, new()
    {
        _systems.Add(new T());
    }

    public TickPipeline AddSystem(ISimulationSystem system)
    {
        _systems.Add(system);
        return this;
    }

    public void Execute(World world, Tick tick)
    {
        foreach (var system in _systems)
            system.Execute(world, tick);
    }

    public static TickPipeline CreateDefault()
    {
        var pipeline = new TickPipeline();
        pipeline.AddSystem(VitalsRegenSystem.Instance);
        pipeline.AddSystem(MovementSystem.Instance);
        pipeline.AddSystem(NpcBrainSystem.Instance);
        pipeline.AddSystem(CombatSystem.Instance);
        pipeline.AddSystem(LevelingSystem.Instance);
        pipeline.AddSystem(DeathSystem.Instance);
        pipeline.AddSystem(GroundItemSystem.Instance);
        pipeline.AddSystem(EquipmentSystem.Instance);
        pipeline.AddSystem(InventorySystem.Instance);
        pipeline.AddSystem(HotbarSystem.Instance);
        pipeline.AddSystem(TradeSystem.Instance);
        pipeline.AddSystem(ShopSystem.Instance);
        pipeline.AddSystem(PartySystem.Instance);
        pipeline.AddSystem(SpawnSystem.Instance);
        pipeline.AddSystem(new ReplicationSystem(
            PlayerSender.Instance, NpcSender.Instance,
            MapSender.Instance));
        return pipeline;
    }
}
