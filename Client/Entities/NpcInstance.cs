using System;
using Arch.Core;
using CryBits.Client.Components;
using CryBits.Client.Spawners;
using CryBits.Client.Worlds;
using CryBits.Entities.Npc;

namespace CryBits.Client.Entities;

internal class NpcInstance : Character
{
    public Npc Data { get; set; }

    // ECS Entity
    public Entity Entity = Entity.Null;

    public void Logic()
    {
        if (Hurt + 325 < Environment.TickCount) Hurt = 0;

        ProcessMovement();

        SyncEntity();
    }

    private void SyncEntity()
    {
        var world = GameContext.Instance.World;

        if (Entity == Entity.Null) Entity = NpcSpawner.Spawn(world, this);

        // Sync Transform
        ref var transform = ref world.Get<TransformComponent>(Entity);
        transform.X = PixelX;
        transform.Y = PixelY;
    }
}
