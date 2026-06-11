using CryBits.Server.Entities;
using CryBits.Server.Logic;
using CryBits.Server.Network.Senders;
using System;

namespace CryBits.Server.Systems;

/// <summary>
/// Tick-driven system that handles map item respawning.
/// Called every 500ms from the main loop; the actual respawn fires every 300s
/// controlled by <see cref="Loop.TimerMapItems"/>.
/// </summary>
internal sealed class MapItemSystem(MapSender mapSender)
{
    public static MapItemSystem Instance { get; } = new(MapSender.Instance);

    /// <summary>
    /// Clears and respawns all static map items when the 300-second timer has elapsed.
    /// No-ops if the map has no players or the timer has not fired yet.
    /// </summary>
    public void Tick(MapInstance mapInstance)
    {
        if (!mapInstance.HasPlayers()) return;
        if (Environment.TickCount64 <= Loop.TimerMapItems + 300000) return;

        mapInstance.Item = [];
        mapInstance.SpawnItems();
        mapSender.MapItems(mapInstance);
    }
}
