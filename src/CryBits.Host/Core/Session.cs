using CryBits.Simulation.State;
using LiteNetLib;

namespace CryBits.Host.Core;

/// <summary>
/// Represents a single connected client session: the network peer,
/// the authenticated account, and the active character (if any).
/// </summary>
internal sealed class Session(NetPeer connection)
{
    /// <summary>Underlying network connection for this session.</summary>
    public NetPeer Connection { get; } = connection;

    /// <summary>The authenticated account, or <see langword="null"/> before login.</summary>
    public Account? Account { get; set; }

    /// <summary>Whether the session is currently using the map / content editor.</summary>
    public bool InEditor { get; set; }

    /// <summary>
    /// The player character entity ID that is currently in the world, or
    /// <see langword="null"/> when the session is at the character-selection screen.
    /// </summary>
    public EntityId? Character { get; set; }

    /// <summary>
    /// <see langword="true"/> when the session has an active character in the world.
    /// </summary>
    public bool IsPlaying => Character is not null;
}
