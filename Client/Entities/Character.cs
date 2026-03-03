using Arch.Core;

namespace CryBits.Client.Entities;

internal abstract class Character
{
    // ECS entity handle. Assigned on spawn, cleared on despawn.
    public Entity Entity = Entity.Null;
}
