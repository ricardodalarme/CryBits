using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;
using CryBits.Server.Simulation.Core;
using CryBits.Server.World;
using System;

namespace CryBits.Server.Systems.Inventory;

internal sealed class GroundItemSystem(MapSender mapSender) : ISimulationSystem
{
    public static GroundItemSystem Instance { get; } = new(MapSender.Instance);

    private long _timer;

    public void Execute(GameWorld world, Tick tick)
    {
        if (Environment.TickCount64 <= _timer + 300000) return;
        _timer = Environment.TickCount64;

        foreach (var map in world.Maps.Values)
        {
            if (!map.HasPlayers()) continue;

            map.Item = [];
            map.SpawnItems();
            mapSender.MapItems(map);
        }
    }
}
