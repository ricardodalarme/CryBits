using CryBits.Enums;

namespace CryBits.Client.Components.Movement;

/// <summary>
/// Holds all client-side movement interpolation state for a character entity.
///
/// Separation of concerns:
///   <c>TileX</c> / <c>TileY</c> / <c>Direction</c> — server-authoritative grid values,
///   written by network packet handlers and the local player input path.
///
///   <c>OffsetX</c> / <c>OffsetY</c> — sub-tile pixel offset computed every game tick by
///   <c>CharacterMovementSystem</c>. They start at ±Grid when movement begins and slide
///   toward 0 as the character walks across the tile boundary.
///
///   <c>MovementState</c> — current locomotion mode. Set to <c>Stopped</c> by the system
///   when the offset reaches zero.
/// </summary>
internal struct CharacterMovementComponent
{
    /// <summary>Current tile X coordinate (server-authoritative).</summary>
    public byte TileX;
    /// <summary>Current tile Y coordinate (server-authoritative).</summary>
    public byte TileY;

    /// <summary>Sub-tile pixel offset on the X axis. Managed by <c>CharacterMovementSystem</c>.</summary>
    public short OffsetX;
    /// <summary>Sub-tile pixel offset on the Y axis. Managed by <c>CharacterMovementSystem</c>.</summary>
    public short OffsetY;

    /// <summary>Current locomotion state. Walking/Moving drives the offset; Stopped freezes it.</summary>
    public CryBits.Enums.Movement MovementState;

    /// <summary>Facing direction. Written by server packets and local player input.</summary>
    public Direction Direction;
}
