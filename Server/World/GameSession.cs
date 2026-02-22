using System.Collections.Generic;
using CryBits.Enums;
using CryBits.Server.Entities;
using LiteNetLib;

namespace CryBits.Server.World;

/// <summary>
/// Represents a single connected client session: the network peer, authentication
/// state, and the active character (if any).
/// </summary>
internal sealed class GameSession(NetPeer connection)
{
    /// <summary>Underlying network connection for this session.</summary>
    public NetPeer Connection { get; } = connection;

    /// <summary>Account username supplied during login.</summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>BCrypt hash of the account password.</summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>Access level granted to the session after authentication.</summary>
    public Access AccessLevel { get; set; }

    /// <summary>Whether the session is currently using the map / content editor.</summary>
    public bool InEditor { get; set; }

    /// <summary>
    /// The player character that is currently in the world, or
    /// <see langword="null"/> when the session is at the character-selection screen.
    /// </summary>
    public Player? Character { get; set; }

    /// <summary>Character slots shown on the character-selection screen.</summary>
    public List<CharacterSlot> Characters { get; set; } = [];

    /// <summary>
    /// <see langword="true"/> when the session has an active character in the world.
    /// </summary>
    public bool IsPlaying => Character is not null;

    /// <summary>Lightweight descriptor used on the character-selection screen.</summary>
    public struct CharacterSlot
    {
        public string Name;
        public short TextureNum;
    }
}
