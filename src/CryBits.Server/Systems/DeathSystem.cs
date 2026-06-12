using CryBits.Definitions.Characters;
using CryBits.Definitions.Common;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;
using CryBits.Server.Simulation.Core;
using CryBits.Server.Simulation.Events;
using CryBits.Server.World;
using System;
using System.Linq;

namespace CryBits.Server.Systems;

internal sealed class DeathSystem(PlayerSender playerSender) : ISimulationSystem
{
    public static DeathSystem Instance { get; } = new(PlayerSender.Instance);

    public void Execute(GameWorld world, Tick tick)
    {
        foreach (var ev in tick.Events.Events.ToArray())
        {
            if (ev is not EntityDiedEvent died) continue;
            if (died.Entity is not Player player) continue;

            for (byte n = 0; n < (byte)Vital.Count; n++)
                player.Vital[n] = player.MaxVital(n);

            playerSender.PlayerVitals(player);

            player.Experience /= 10;
            playerSender.PlayerExperience(player);

            player.Direction = (Direction)player.Class.SpawnDirection;
            MovementSystem.Instance.Warp(player,
                world.Maps.Get(player.Class.SpawnMap.Id),
                player.Class.SpawnX,
                player.Class.SpawnY);
        }
    }
}
