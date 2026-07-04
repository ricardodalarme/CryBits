using CryBits.Client.Components;
using CryBits.Client.Core;
using CryBits.Client.Managers;
using CryBits.Client.Network.Senders;
using CryBits.Client.Worlds;
using CryBits.Simulation.Components;
using CryBits.Simulation.Intents;
using CryBits.Simulation.Spatial;
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

        if (inputManager.IsScancodePressed(Keyboard.Scancode.Up)) Move(entity, Direction.Up, movement);
        else if (inputManager.IsScancodePressed(Keyboard.Scancode.Down)) Move(entity, Direction.Down, movement);
        else if (inputManager.IsScancodePressed(Keyboard.Scancode.Left)) Move(entity, Direction.Left, movement);
        else if (inputManager.IsScancodePressed(Keyboard.Scancode.Right)) Move(entity, Direction.Right, movement);
    }

    private void Move(EntityId entity, Direction direction, MovementComponent movement)
    {
        var desired = inputManager.IsKeyPressed(Keyboard.Key.LShift)
            ? MovementState.Moving
            : MovementState.Walking;

        intentSender.Send(new MoveIntent(default, direction, desired));

        var nextX = direction switch
        {
            Direction.Right => movement.TileX + 1,
            Direction.Left => movement.TileX - 1,
            _ => movement.TileX
        };
        var nextY = direction switch
        {
            Direction.Down => movement.TileY + 1,
            Direction.Up => movement.TileY - 1,
            _ => movement.TileY
        };

        var map = context.CurrentMap;
        if (map != null && ChunkGrid.IsTileBlocked(map, nextX, nextY))
        {
            context.World.Set(entity, movement with { Direction = direction });
            return;
        }
        if (HasSolidEntityAt(nextX, nextY))
        {
            context.World.Set(entity, movement with { Direction = direction });
            return;
        }

        var speed = desired == MovementState.Moving ? RunSpeedPixelsPerSecond : WalkSpeedPixelsPerSecond;

        var (offsetX, offsetY, tileX, tileY) = direction switch
        {
            Direction.Up => (0f, Grid, movement.TileX, movement.TileY - 1),
            Direction.Down => (0f, -Grid, movement.TileX, movement.TileY + 1),
            Direction.Right => (-Grid, 0f, movement.TileX + 1, movement.TileY),
            Direction.Left => (Grid, 0f, movement.TileX - 1, movement.TileY),
            _ => (0f, 0f, movement.TileX, movement.TileY)
        };

        context.World.Set(entity, new MovementComponent(tileX, tileY, offsetX, offsetY, speed, desired, direction));
    }

    private bool HasSolidEntityAt(int tileX, int tileY)
    {
        var playerPos = context.World.Get<Position>(context.LocalPlayer.Entity!.Value);
        if (playerPos == null) return false;

        return ChunkGrid.FindAt<CollidableTag>(context.World, playerPos.MapId, tileX, tileY).HasValue;
    }
}
