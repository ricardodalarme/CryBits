using Arch.Core;
using CryBits.Client.Entities;

namespace CryBits.Client.Worlds;

/// <summary>
/// Single-instance runtime state for the game client.
/// </summary>
internal sealed class GameContext
{
    /// <summary>Singleton instance of the game context.</summary>
    public static GameContext Instance { get; } = new();

    /// <summary>All live game entities and their components.</summary>
    public Arch.Core.World World { get; } = Arch.Core.World.Create();

    /// <summary>Current map instance.</summary>
    public MapInstance CurrentMap;

    /// <summary>Tracks the local player entity and components.</summary>
    public LocalPlayer LocalPlayer { get; set; } = new(Entity.Null);

    /// <summary>
    /// Fully reset world state on disconnect: destroys all entities,
    /// clears map data, and zeroes out slot arrays.
    /// </summary>
    public void Reset()
    {
        World.Clear();
        LocalPlayer = new LocalPlayer(Entity.Null);
    }
}
