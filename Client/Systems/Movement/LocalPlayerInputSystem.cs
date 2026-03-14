using Arch.Core;
using Arch.System;
using CryBits.Client.Components.Movement;
using CryBits.Client.Managers;
using CryBits.Client.Network.Senders;
using CryBits.Client.UI.Game.Views;
using CryBits.Client.Worlds;
using CryBits.Enums;
using SFML.Window;
using System;
using static CryBits.Globals;
using MovementState = CryBits.Enums.Movement;

namespace CryBits.Client.Systems.Movement;

/// <summary>
/// Polls keyboard state to drive local-player movement and attacks.
/// Runs every frame; uses an internal 30 ms throttle to match the original tick rate.
/// </summary>
internal class LocalPlayerInputSystem(World world, GameContext context, InputManager inputManager, PlayerSender playerSender) : BaseSystem<World, float>(world)
{
    private int _nextInputMs;

    public override void Update(in float t)
    {
        var entity = context.LocalPlayer.Entity;
        if (entity == Entity.Null || !World.IsAlive(entity)) return;

        // Throttle movement + attack to ~33 Hz (matches legacy Me.Logic timer)
        if (Environment.TickCount < _nextInputMs) return;
        _nextInputMs = Environment.TickCount + 30;

        CheckMovement(entity);
        CheckAttack(entity);
    }

    private void CheckMovement(Entity entity)
    {
        ref var movement = ref World.Get<MovementComponent>(entity);
        if (movement.MovementState != MovementState.Stopped) return;

        if (inputManager.IsScancodePressed(Keyboard.Scancode.Up)) Move(Direction.Up, ref movement);
        else if (inputManager.IsScancodePressed(Keyboard.Scancode.Down)) Move(Direction.Down, ref movement);
        else if (inputManager.IsScancodePressed(Keyboard.Scancode.Left)) Move(Direction.Left, ref movement);
        else if (inputManager.IsScancodePressed(Keyboard.Scancode.Right)) Move(Direction.Right, ref movement);
    }

    private void Move(Direction direction, ref MovementComponent movement)
    {
        movement.Direction = direction;

        var desired = inputManager.IsKeyPressed(Keyboard.Key.LShift)
            ? MovementState.Moving
            : MovementState.Walking;

        playerSender.PlayerMove(direction, desired);

        if (context.CurrentMap.TileBlocked(movement.TileX, movement.TileY, direction)) return;

        movement.MovementState = desired;
        movement.SpeedPixelsPerSecond = desired == MovementState.Moving
            ? RunSpeedPixelsPerSecond
            : WalkSpeedPixelsPerSecond;

        switch (direction)
        {
            case Direction.Up: movement.OffsetY = Grid; movement.TileY--; break;
            case Direction.Down: movement.OffsetY = -Grid; movement.TileY++; break;
            case Direction.Right: movement.OffsetX = -Grid; movement.TileX++; break;
            case Direction.Left: movement.OffsetX = Grid; movement.TileX--; break;
        }
    }

    private void CheckAttack(Entity entity)
    {
        ref var state = ref World.Get<CharacterStateComponent>(entity);

        if (state.AttackTimer + AttackSpeed < Environment.TickCount)
        {
            state.AttackTimer = 0;
            state.IsAttacking = false;
        }

        if (!inputManager.IsKeyPressed(Keyboard.Key.LControl)) return;
        if (state.AttackTimer > 0) return;
        if (TradeView.Panel.Visible) return;
        if (ShopView.Panel.Visible) return;

        state.AttackTimer = Environment.TickCount;
        state.IsAttacking = true;
        playerSender.PlayerAttack();
    }
}
