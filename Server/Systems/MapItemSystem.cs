using System;
using System.Collections.Generic;
using CryBits.Server.Entities.TempMap;
using CryBits.Server.Logic;
using CryBits.Server.Network.Senders;

namespace CryBits.Server.Systems;

/// <summary>
/// Tick-driven system that handles map item respawning.
/// Called every 500ms from the main loop; the actual respawn fires every 300s
/// controlled by <see cref="Loop.TimerMapItems"/>.
/// </summary>
internal static class MapItemSystem
{
    /// <summary>
    /// Clears and respawns all static map items when the 300-second timer has elapsed.
    /// No-ops if the map has no players or the timer has not fired yet.
    /// </summary>
    public static void Tick(TempMap map)
    {
        if (!map.HasPlayers()) return;
        if (Environment.TickCount64 <= Loop.TimerMapItems + 300000) return;

        map.Item = new List<TempMapItems>();
        map.SpawnItems();
        MapSender.MapItems(map);
    }
}
