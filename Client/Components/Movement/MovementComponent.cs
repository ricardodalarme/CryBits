using CryBits.Enums;

namespace CryBits.Client.Components.Movement;

/// <summary>
/// Holds all client-side movement interpolation state for a character entity.
/// </summary>
internal struct MovementComponent
{
    /// <summary>Current tile X coordinate (server-authoritative).</summary>
    public byte TileX;
    /// <summary>Current tile Y coordinate (server-authoritative).</summary>
    public byte TileY;

    /// <summary>Sub-tile pixel offset on the X axis, in world pixels. Float for smooth dt-based interpolation.</summary>
    public float OffsetX;
    /// <summary>Sub-tile pixel offset on the Y axis, in world pixels. Float for smooth dt-based interpolation.</summary>
    public float OffsetY;

    /// <summary>Interpolation speed in world-pixels per second. Written by spawners and movement packets.</summary>
    public float SpeedPixelsPerSecond;

    /// <summary>Current locomotion state. Walking/Moving drives the offset; Stopped freezes it.</summary>
    public CryBits.Enums.Movement MovementState;

    /// <summary>Facing direction. Written by server packets and local player input.</summary>
    public Direction Direction;
}
