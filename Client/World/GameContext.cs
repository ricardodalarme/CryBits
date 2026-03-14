using Arch.Core;
using CryBits.Client.Components.Character;
using CryBits.Client.Components.Player;

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

    /// <summary>Name of the local player. Set on Join, cleared on Reset.</summary>
    public string? LocalPlayerName;

    /// <summary>Tracks the local player entity and components.</summary>
    public LocalPlayer LocalPlayer { get; set; }

    private static readonly QueryDescription _playerNameQuery =
        new QueryDescription().WithAll<NameComponent, PlayerTagComponent>();

    internal GameContext()
    {
        LocalPlayer = new LocalPlayer(World, Entity.Null);
    }

    /// <summary>Returns the ECS entity whose NameComponent matches <paramref name="name"/>, or Entity.Null.</summary>
    public Entity GetPlayerEntity(string name)
    {
        var found = Entity.Null;
        World.Query(in _playerNameQuery, (Entity e, ref NameComponent n) =>
        {
            if (n.Value == name) found = e;
        });
        return found;
    }

    /// <summary>
    /// Fully reset world state on disconnect: destroys all entities,
    /// clears map data, and zeroes out slot arrays.
    /// </summary>
    public void Reset()
    {
        World.Clear();
        CurrentMap = null!;
        LocalPlayer = new LocalPlayer(World, Entity.Null);
        LocalPlayerName = null;
    }
}
