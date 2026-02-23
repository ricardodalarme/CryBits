using CryBits.Client.ECS.Systems;
using CryBits.Enums;

namespace CryBits.Client.ECS.Components;

/// <summary>
/// Tracks whether and how an entity is currently moving between tiles.
/// The <see cref="MovementSystem"/> reads this each tick and advances
/// <see cref="TransformComponent.PixelOffsetX"/> / <see cref="TransformComponent.PixelOffsetY"/>
/// until the entity reaches its destination tile, then resets to <see cref="Movement.Stopped"/>.
/// </summary>
public sealed class MovementComponent : IComponent
{
    public Movement Current { get; set; } = Movement.Stopped;
}
