using CryBits.Server.Entities;
using CryBits.Server.Simulation.Core;
using System;
using System.Collections.Generic;

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

    public GameWorld() => Current = this;
}
