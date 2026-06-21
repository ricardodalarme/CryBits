using CryBits.Definitions.Common;

namespace CryBits.Client.Components;

public sealed class MovementComponent
{
    public byte TileX { get; set; }
    public byte TileY { get; set; }
    public float OffsetX { get; set; }
    public float OffsetY { get; set; }
    public float SpeedPixelsPerSecond { get; set; }
    public CryBits.Definitions.Common.Movement MovementState { get; set; }
    public Direction Direction { get; set; }

    public bool IsMoving => OffsetX != 0f || OffsetY != 0f;
}
