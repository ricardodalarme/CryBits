using CryBits.Host.Replication;
using CryBits.Simulation.State;

namespace CryBits.Host.Core;

/// <summary>
/// Represents a single connected client session: the transport session ID,
/// the authenticated account, and the active character (if any).
/// </summary>
public sealed class Session(Guid id)
{
    /// <summary>Unique identifier for this session, assigned by the transport layer.</summary>
    public Guid Id { get; } = id;

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

    /// <summary>Replication state for delta encoding, or <see langword="null"/> before joining.</summary>
    internal ObserverState? ReplicationState { get; set; }
}
