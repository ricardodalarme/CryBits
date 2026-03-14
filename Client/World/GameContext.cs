using Arch.Core;
using System;
using System.Collections.Generic;

namespace CryBits.Client.Worlds;

/// <summary>
/// Single-instance runtime state for the game client.
/// </summary>
internal sealed class GameContext
{
    /// <summary>Singleton instance of the game context.</summary>
    public static GameContext Instance { get; } = new();

    /// <summary>All live game entities and their components.</summary>
    public World World { get; } = World.Create();

    /// <summary>Current map instance.</summary>
    public ClientMap CurrentMap = null!;

    /// <summary>Tracks the local player entity and components.</summary>
    public LocalPlayer LocalPlayer { get; set; }

    private readonly Dictionary<Guid, Entity> _entityById = [];

    internal GameContext()
    {
        LocalPlayer = new LocalPlayer(World, Entity.Null);
    }

    /// <summary>Registers a network entity so it can be found by ID in O(1).</summary>
    public void RegisterNetworkEntity(Guid id, Entity entity) => _entityById[id] = entity;

    /// <summary>Removes a network entity registration (call before World.Destroy).</summary>
    public void UnregisterNetworkEntity(Guid id) => _entityById.Remove(id);

    /// <summary>Returns the ECS entity with the given network ID, or Entity.Null if not found.</summary>
    public Entity GetNetworkEntity(Guid id) => _entityById.TryGetValue(id, out var e) ? e : Entity.Null;

    /// <summary>
    /// Fully reset world state on disconnect: destroys all entities,
    /// clears map data, and zeroes out slot arrays.
    /// </summary>
    public void Reset()
    {
        World.Clear();
        _entityById.Clear();
        CurrentMap = null!;
        LocalPlayer = new LocalPlayer(World, Entity.Null);
    }
}
