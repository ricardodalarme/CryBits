using CryBits.Definitions.Characters;
using CryBits.Definitions.Common;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;
using CryBits.Server.Simulation.Core;
using CryBits.Simulation.Events;
using CryBits.Server.Systems.Movement;
using CryBits.Server.World;
using System.Linq;
using CryBits.Simulation.Core;

namespace CryBits.Server.Systems.Combat;

internal sealed class DeathSystem(PlayerSender playerSender) : ISimulationSystem
{
    public static DeathSystem Instance { get; } = new(PlayerSender.Instance);

    public void Execute(GameWorld world, Tick tick)
    {
        foreach (var ev in tick.Events.Events.ToArray())
        {
            if (ev is not EntityDiedEvent died) continue;
            if (!died.EntityIsPlayer) continue;
            var player = world.FindPlayer(died.EntityId);
            if (player == null) continue;

            for (byte n = 0; n < (byte)Vital.Count; n++)
                player.Vital[n] = player.MaxVital(n);

            playerSender.PlayerVitals(player);

            player.Experience /= 10;
            playerSender.PlayerExperience(player);

            player.Direction = (Direction)player.Class.SpawnDirection;
            MovementSystem.Instance.Warp(player,
                world.Maps.Get(player.Class.SpawnMapId),
                player.Class.SpawnX,
                player.Class.SpawnY);
        }
    }
}
