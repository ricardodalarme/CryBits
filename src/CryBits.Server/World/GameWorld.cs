using CryBits.Server.Entities;
using CryBits.Simulation.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CryBits.Server.World;

/// <summary>
/// Single container for all live server-side game state.
/// </summary>
internal sealed class GameWorld
{
    /// <summary>
    /// Transitional static accessor for the current game world instance.
    /// </summary>
    public static GameWorld Current { get; private set; } = null!;

    /// <summary>
    /// Live map instances keyed by map ID.
    /// </summary>
    public Dictionary<Guid, MapInstance> Maps { get; } = [];

    /// <summary>
    /// List of all active sessions on the server.
    /// </summary>
    public List<GameSession> Sessions { get; } = [];

    /// <summary>
    /// The tick currently being processed. Set before HandleData and cleared after
    /// the pipeline executes, so any system can emit events via CurrentTick.Events.Emit().
    /// </summary>
    public Tick? CurrentTick { get; set; }

    /// <summary>Finds a live player by their unique identifier.</summary>
    public Player? FindPlayer(Guid id) =>
        Sessions.Find(s => s.IsPlaying && s.Character!.Id == id)?.Character;

    /// <summary>Finds a playing player by name.</summary>
    /// <param name="name">Player name to search for.</param>
    /// <returns>The Player instance if found; otherwise null.</returns>
    public Player? FindPlayer(string name) =>
        Sessions.Find(x => x.IsPlaying && x.Character!.Name.Equals(name))?.Character;

    /// <summary>Finds a live NPC instance by its unique identifier across all maps.</summary>
    public NpcInstance? FindNpcInstance(Guid id)
    {
        foreach (var map in Maps.Values)
        {
            var npc = Array.Find(map.Npc, n => n.Id == id);
            if (npc != null) return npc;
        }
        return null;
    }

    public GameWorld() => Current = this;
}
