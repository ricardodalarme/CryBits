using System;
using Arch.Core;
using CryBits.Client.Components.Combat;
using CryBits.Client.Components.Movement;
using CryBits.Client.Worlds;
using CryBits.Enums;

namespace CryBits.Client.Entities;

internal abstract class Character
{
    // ECS Entity handle — Entity.Null until the first Logic() call spawns it.
    public Entity Entity = Entity.Null;

    // Server-authoritative fields. Written by network packet handlers.
    public byte X;
    public byte Y;
    public Direction Direction;
    public bool Attacking;
    public int Hurt;
    public int AttackTimer;
    public short[] Vital = new short[(byte)Enums.Vital.Count];

    /// <summary>
    /// Pushes server-authoritative state into ECS components every game tick.
    /// </summary>
    protected void Update()
    {
        if (Hurt + 325 < Environment.TickCount) Hurt = 0;

        var world = GameContext.Instance.World;

        // Tile coords + direction — read by CharacterMovementSystem to compute Transform.
        ref var movement = ref world.Get<MovementComponent>(Entity);
        movement.TileX = X;
        movement.TileY = Y;
        movement.Direction = Direction;

        // Combat state — read by CharacterAnimationControllerSystem for the attack frame.
        ref var state = ref world.Get<CharacterStateComponent>(Entity);
        state.IsAttacking = Attacking;
        state.AttackTimer = AttackTimer;

        // Hurt state drives the damage-tint flash on the sprite.
        ref var tint = ref world.Get<DamageTintComponent>(Entity);
        tint.IsHurt = Hurt > 0;

        // Current vitals — NPC handlers can replace the array ref entirely so we always
        // copy element-by-element rather than sharing a reference.
        ref var vitals = ref world.Get<VitalsComponent>(Entity);
        Array.Copy(Vital, vitals.Current, Vital.Length);
    }
}
