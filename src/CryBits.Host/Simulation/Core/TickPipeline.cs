using CryBits.Host.Network.Senders;
using CryBits.Host.Systems.Combat;
using CryBits.Host.Systems.Inventory;
using CryBits.Host.Systems.Movement;
using CryBits.Host.Systems.Npc;
using CryBits.Host.Systems.Party;
using CryBits.Host.Systems.Progression;
using CryBits.Host.Systems.Shops;
using CryBits.Host.Systems.Spawning;
using CryBits.Host.Systems.Trade;
using CryBits.Simulation.Core;
using System.Collections.Generic;

namespace CryBits.Host.Simulation.Core;

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
