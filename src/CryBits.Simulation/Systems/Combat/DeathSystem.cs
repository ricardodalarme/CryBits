using CryBits.Definitions.Common;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Events;
using static CryBits.Simulation.SimulationConstants;

namespace CryBits.Simulation.Systems.Combat;

public sealed class DeathSystem : ISimulationSystem
{
    public void Execute(World world, Tick tick)
    {
        var events = tick.Events.Events;
        var count = events.Count;
        for (var i = 0; i < count; i++)
        {
            var ev = events[i];
            if (ev is PlayerDiedEvent died)
                HandlePlayerDeath(world, tick, died);
            else if (ev is NpcDiedEvent npcDied)
                HandleNpcDeath(world, tick, npcDied);
        }
    }

    private void HandlePlayerDeath(World world, Tick tick, PlayerDiedEvent died)
    {
        if (!world.Has<PlayerTag>(died.EntityId)) return;

        if (!world.IsAlive(died.EntityId)) return;
        var vitals = world.Get<Vitals>(died.EntityId)!;
        var pos = world.Get<Position>(died.EntityId)!;
        var appearance = world.Get<PlayerAppearance>(died.EntityId)!;
        var playerClass = world.Catalog.Classes.Get(appearance.ClassId);
        if (playerClass is null) return;

        world.Set(died.EntityId, new Vitals(Hp: vitals.MaxHp, Mp: vitals.MaxMp, MaxHp: vitals.MaxHp, MaxMp: vitals.MaxMp));

        var oldMapId = pos.MapId;

        world.Update<Position>(died.EntityId, p => new Position(Direction: (Direction)playerClass.SpawnDirection, MapId: playerClass.SpawnMapId, X: playerClass.SpawnX, Y: playerClass.SpawnY));

        if (oldMapId != playerClass.SpawnMapId)
            world.Set(died.EntityId, new MapLoadingTag());

        tick.Events.Emit(new PlayerWarpedEvent(tick.TickNumber, died.EntityId, oldMapId, playerClass.SpawnMapId, true));
    }

    private void HandleNpcDeath(World world, Tick tick, NpcDiedEvent died)
    {
        if (!world.IsAlive(died.EntityId)) return;
 
        var pos = world.Get<Position>(died.EntityId)!;

        var npcData = world.Catalog.Npcs.Get(died.NpcDefId);
        if (npcData == null) return;

        for (byte d = 0; d < npcData.Drop.Count; d++)
            if (npcData.Drop[d].ItemId != Guid.Empty)
                if (Random.Shared.Next(1, 99) <= npcData.Drop[d].Chance)
                    tick.Events.Emit(new LootDroppedEvent(tick.TickNumber, pos.MapId, pos.X, pos.Y, npcData.Drop[d].ItemId, npcData.Drop[d].Amount, tick.TickNumber + GroundItemDespawnTicks));

        world.Destroy(died.EntityId);
    }
}
