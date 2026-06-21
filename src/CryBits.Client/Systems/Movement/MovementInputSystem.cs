using CryBits.Client.Components;
using CryBits.Client.Core;
using CryBits.Client.Managers;
using CryBits.Client.Network.Senders;
using CryBits.Client.Worlds;
using CryBits.Simulation.Intents;
using CryBits.Simulation.State;
using SFML.Window;
using static CryBits.Definitions.Globals;
using Direction = CryBits.Definitions.Common.Direction;
using MovementState = CryBits.Definitions.Common.Movement;

namespace CryBits.Client.Systems.Movement;

internal sealed class MovementInputSystem(GameContext context, InputManager inputManager, IntentSender intentSender) : IClientSystem
{
    private const float ThrottleInterval = 0.030f;

    private float _inputThrottle;

    public void Update(float t)
    {
        var localPlayer = context.LocalPlayer;
        if (localPlayer is null) return;

        var entity = localPlayer.Entity;
        if (entity == null || !context.World.IsAlive(entity.Value)) return;

        _inputThrottle += t;
        if (_inputThrottle < ThrottleInterval) return;
        _inputThrottle -= ThrottleInterval;

        CheckMovement(entity.Value);
    }

    private void CheckMovement(EntityId entity)
    {
        var movement = context.World.Get<MovementComponent>(entity);
        if (movement == null || movement.MovementState != MovementState.Stopped) return;

        if (inputManager.IsScancodePressed(Keyboard.Scancode.Up)) Move(Direction.Up, movement);
        else if (inputManager.IsScancodePressed(Keyboard.Scancode.Down)) Move(Direction.Down, movement);
        else if (inputManager.IsScancodePressed(Keyboard.Scancode.Left)) Move(Direction.Left, movement);
        else if (inputManager.IsScancodePressed(Keyboard.Scancode.Right)) Move(Direction.Right, movement);
    }

    private void Move(Direction direction, MovementComponent movement)
    {
        movement.Direction = direction;

        var desired = inputManager.IsKeyPressed(Keyboard.Key.LShift)
            ? MovementState.Moving
            : MovementState.Walking;

        intentSender.Send(new MoveIntent(default, direction, desired));

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
