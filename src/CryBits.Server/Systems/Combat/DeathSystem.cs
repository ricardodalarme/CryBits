using CryBits.Definitions.Catalog;
using CryBits.Definitions.Characters;
using CryBits.Definitions.Common;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;
using CryBits.Server.Simulation.Core;
using CryBits.Server.Simulation.State;
using CryBits.Server.Simulation.State.Components;
using CryBits.Simulation.Events;
using CryBits.Server.Systems.Movement;
using CryBits.Server.World;
using System.Linq;
using CryBits.Simulation.Core;

namespace CryBits.Server.Systems.Combat;

internal sealed class DeathSystem(PlayerSender playerSender, DefinitionCatalog catalog) : ISimulationSystem
{
    private readonly DefinitionCatalog _catalog = catalog;
    public static DeathSystem Instance { get; } = new(PlayerSender.Instance, DefinitionCatalog.Instance);

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

            playerSender.PlayerVitals(playerId.Value);

            stats.Experience /= 10;
            playerSender.PlayerExperience(playerId.Value);

            pos.Direction = (Direction)playerClass.SpawnDirection;
            MovementSystem.Instance.Warp(playerId.Value,
                world.Maps.Get(playerClass.SpawnMapId),
                playerClass.SpawnX,
                playerClass.SpawnY);
        }
    }
}
