using CryBits.Server.Entities;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.State;
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
    /// Entity registry holding all live entity states (players and NPCs).
    /// </summary>
    public EntityRegistry Entities { get; } = new();

    /// <summary>
    /// Maps entity IDs to their network sessions (players only).
    /// </summary>
    public SessionMap SessionMap { get; } = new();

    /// <summary>
    /// Tracks which (EntityId, ComponentType) pairs were mutated each tick.
    /// </summary>
    public DirtyTracking Dirty { get; } = new();

    /// <summary>
    /// The tick currently being processed. Set before HandleData and cleared after
    /// the pipeline executes, so any system can emit events via CurrentTick.Events.Emit().
    /// </summary>
    public Tick? CurrentTick { get; set; }

    /// <summary>Finds a playing player by name.</summary>
    public EntityId? FindPlayer(string name)
    {
        foreach (var state in Entities.All)
        {
            if (!state.Has<PlayerTag>()) continue;
            var appearance = state.Get<PlayerAppearance>()!;
            if (appearance.Name.Equals(name))
                return state.Id;
        }
        return null;
    }

    /// <summary>Finds a live NPC instance by its unique identifier across all maps.</summary>
    public EntityId? FindNpcInstance(Guid id)
    {
        var entityId = new EntityId(id);
        var state = Entities.Get(entityId);
        if (state != null && state.Has<NpcTag>()) return entityId;
        return null;
    }

    /// <summary>Finds a live player by their entity ID value (Guid).</summary>
    public EntityId? FindPlayerByValue(Guid id)
    {
        var entityId = new EntityId(id);
        var state = Entities.Get(entityId);
        if (state != null && state.Has<PlayerTag>()) return entityId;
        return null;
    }

    public GameWorld() => Current = this;
}
