using CryBits.Definitions.Common;

namespace CryBits.Client.Components;

public sealed record MovementComponent(int TileX, int TileY, float OffsetX, float OffsetY, float SpeedPixelsPerSecond, Movement MovementState, Direction Direction)
{
    public bool IsMoving => OffsetX != 0f || OffsetY != 0f;
}
