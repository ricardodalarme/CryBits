using System;
using System.Collections.Generic;
using CryBits.Server.Entities;

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
    /// List of all active accounts on the server.
    /// </summary>
    public List<Account> Accounts { get; } = [];

    public GameWorld() => Current = this;
}
