using CryBits.Client.Components;
using CryBits.Client.Core;
using CryBits.Client.Input;
using CryBits.Client.Network.Senders;
using CryBits.Client.Replication;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Intents;
using CryBits.Simulation.Spatial;
using Microsoft.Xna.Framework.Input;
using static CryBits.Definitions.Globals;
using Direction = CryBits.Definitions.Common.Direction;
using MovementState = CryBits.Definitions.Common.Movement;

namespace CryBits.Client.Systems.Movement;

internal sealed class MovementInputSystem(
    ReplicationState replication,
    InputManager inputManager,
    IntentSender intentSender) : IClientSystem
{
    private const float ThrottleInterval = 0.030f;

    private float _inputThrottle;

    public void Update(World world, float t)
    {
        var entity = replication.LocalPlayerEntity;
        if (entity == null || !world.IsAlive(entity.Value)) return;

        _inputThrottle += t;
        if (_inputThrottle < ThrottleInterval) return;
        _inputThrottle -= ThrottleInterval;

        CheckMovement(world, entity.Value);
    }

    private void CheckMovement(World world, EntityId entity)
    {
        var movement = world.Get<MovementComponent>(entity);
        if (movement is not { MovementState: MovementState.Stopped }) return;

        if (inputManager.IsKeyDown(Keys.Up)) Move(world, entity, Direction.Up, movement);
        else if (inputManager.IsKeyDown(Keys.Down)) Move(world, entity, Direction.Down, movement);
        else if (inputManager.IsKeyDown(Keys.Left)) Move(world, entity, Direction.Left, movement);
        else if (inputManager.IsKeyDown(Keys.Right))
            Move(world, entity, Direction.Right, movement);
    }

    private void Move(World world, EntityId entity, Direction direction, MovementComponent movement)
    {
        var desired = inputManager.IsKeyDown(Keys.LeftShift)
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

        var map = world.CurrentMap;
        if (map != null && ChunkGrid.IsTileBlocked(map, nextX, nextY))
        {
            world.Set(entity, movement with { Direction = direction });
            return;
        }

        if (HasSolidEntityAt(world, nextX, nextY))
        {
            world.Set(entity, movement with { Direction = direction });
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

        world.Set(entity, new MovementComponent(tileX, tileY, offsetX, offsetY, speed, desired, direction));
    }

    private bool HasSolidEntityAt(World world, int tileX, int tileY)
    {
        var playerPos = world.Get<Position>(replication.LocalPlayerEntity!.Value);
        if (playerPos == null) return false;

        return ChunkGrid.FindAt<CollidableTag>(world, playerPos.MapId, tileX, tileY).HasValue;
    }
}
