using Arch.Core;
using Arch.System;
using CryBits.Client.Components.Movement;
using CryBits.Client.Managers;
using CryBits.Client.Network.Senders;
using CryBits.Client.Worlds;
using CryBits.Enums;
using SFML.Window;
using static CryBits.Globals;
using MovementState = CryBits.Enums.Movement;

namespace CryBits.Client.Systems.Movement;

/// <summary>
/// Polls keyboard state to drive local-player movement.
/// Runs every frame; uses an internal 30 ms throttle to match the original tick rate.
/// </summary>
internal class MovementInputSystem(World world, GameContext context, InputManager inputManager, PlayerSender playerSender) : BaseSystem<World, float>(world)
{
    /// <summary>Minimum seconds between input polls — ~33 Hz.</summary>
    private const float ThrottleInterval = 0.030f;

    private float _inputThrottle;

    public override void Update(in float t)
    {
        var localPlayer = context.LocalPlayer;
        if (localPlayer is null) return;

        var entity = localPlayer.Entity;
        if (entity == Entity.Null || !World.IsAlive(entity)) return;

        // Throttle movement to ~33 Hz (matches legacy Me.Logic timer).
        // Subtract instead of reset to preserve any accumulated overflow.
        _inputThrottle += t;
        if (_inputThrottle < ThrottleInterval) return;
        _inputThrottle -= ThrottleInterval;

        CheckMovement(entity);
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
}
