using CryBits.Enums;

namespace CryBits.Server.ECS.Components;

/// <summary>Facing direction. Shared by players and NPCs.</summary>
internal sealed class DirectionComponent : ECS.IComponent
{
    public Direction Value;
}
