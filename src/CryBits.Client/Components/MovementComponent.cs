using CryBits.Definitions.Common;

namespace CryBits.Client.Components;

public sealed record class MovementComponent(int TileX, int TileY, float OffsetX, float OffsetY, float SpeedPixelsPerSecond, CryBits.Definitions.Common.Movement MovementState, Direction Direction)
{
    public bool IsMoving => OffsetX != 0f || OffsetY != 0f;
}
