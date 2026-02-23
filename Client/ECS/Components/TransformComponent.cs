using CryBits.Enums;

namespace CryBits.Client.ECS.Components;

/// <summary>
/// World-space position and sub-tile pixel offsets used for smooth scrolling.
///
/// <c>TileX / TileY</c> — the tile the entity occupies (updated as soon as movement begins).
/// <c>PixelOffsetX / PixelOffsetY</c> — fractional displacement toward the previous tile,
/// starting at ±Grid and approaching 0 as the entity slides into place.
/// </summary>
public sealed class TransformComponent : IComponent
{
    public byte TileX { get; set; }
    public byte TileY { get; set; }
    public short PixelOffsetX { get; set; }
    public short PixelOffsetY { get; set; }
    public Direction Direction { get; set; }

    /// <summary>Absolute pixel X position (used by renderers).</summary>
    public int PixelX => TileX * Globals.Grid + PixelOffsetX;

    /// <summary>Absolute pixel Y position (used by renderers).</summary>
    public int PixelY => TileY * Globals.Grid + PixelOffsetY;
}
