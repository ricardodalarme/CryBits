using CryBits.Definitions.Catalog;
using CryBits.Definitions.Characters;
using CryBits.Definitions.Common;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Server.Simulation.Core;
using CryBits.Simulation.Components;
using CryBits.Simulation.Events;
using CryBits.Server.Systems.Movement;
using System.Linq;
using CryBits.Simulation.Core;
using CryBits.Server.Core;

namespace CryBits.Server.Systems.Combat;

internal sealed class DeathSystem(DefinitionCatalog catalog) : ISimulationSystem
{
    private readonly DefinitionCatalog _catalog = catalog;
    public static DeathSystem Instance { get; } = new(DefinitionCatalog.Instance);

    public void Execute(GameWorld world, Tick tick)
    {
        foreach (var ev in tick.Events.Events.ToArray())
        {
            if (ev is not EntityDiedEvent died) continue;
            if (!died.EntityIsPlayer) continue;
            var playerId = world.FindPlayerByValue(died.EntityId);
            if (playerId == null) continue;

            var e = world.Entities.Get(playerId.Value)!;
            var vitals = e.Get<Vitals>()!;
            var pos = e.Get<Position>()!;
            var stats = e.Get<StatBlock>()!;
            var appearance = e.Get<PlayerAppearance>()!;
            var playerClass = _catalog.Classes.Get(appearance.ClassId);

            for (byte n = 0; n < (byte)Vital.Count; n++)
            {
                if (n == 0) vitals.Hp = vitals.MaxHp; else vitals.Mp = vitals.MaxMp;
            }

            world.Dirty.Mark<Vitals>(playerId.Value);

            stats.Experience /= 10;
            world.Dirty.Mark<StatBlock>(playerId.Value);

            pos.Direction = (Direction)playerClass.SpawnDirection;
            MovementSystem.Instance.Warp(playerId.Value,
                world.Maps.Get(playerClass.SpawnMapId),
                playerClass.SpawnX,
                playerClass.SpawnY);
        }
    }
}
