using System;
using System.Collections.Generic;
using Arch.Core;
using CryBits.Client.Components.Character;
using CryBits.Client.Components.Player;
using CryBits.Client.Network.Senders;

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
    public ClientMap CurrentMap;

    /// <summary>Name of the local player. Set on Join, cleared on Reset.</summary>
    public string? LocalPlayerName;

    /// <summary>All received maps keyed by their Guid.</summary>
    public Dictionary<Guid, ClientMap> Maps = [];

    /// <summary>Tracks the local player entity and components.</summary>
    public LocalPlayer LocalPlayer { get; set; }

    private readonly PlayerSender _playerSender = PlayerSender.Instance;

    private static readonly QueryDescription _playerNameQuery =
        new QueryDescription().WithAll<NameComponent, PlayerTagComponent>();

    public GameContext()
    {
        LocalPlayer = CreateLocalPlayer(Entity.Null);
    }

    /// <summary>Creates a new <see cref="LocalPlayer"/> wired to this context's world and player sender.</summary>
    public LocalPlayer CreateLocalPlayer(Entity entity) =>
        new LocalPlayer(entity, World, _playerSender);

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
        LocalPlayer = CreateLocalPlayer(Entity.Null);
        Maps.Clear();
        LocalPlayerName = null;
    }
}
