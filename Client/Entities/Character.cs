using Arch.Core;
using CryBits.Enums;

namespace CryBits.Client.Entities;

internal abstract class Character
{
    // ECS Entity handle — Entity.Null until the first Logic() call spawns it.
    public Entity Entity = Entity.Null;

    // Server-authoritative tile position and facing direction.
    // Still used by handlers, spawners and the movement/tile-blocking systems.
    // Will be fully migrated to MovementComponent in a future step.
    public byte X;
    public byte Y;
    public Direction Direction;
}
