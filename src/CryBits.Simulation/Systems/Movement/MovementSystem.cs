using CryBits.Definitions.Common;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Events;
using CryBits.Simulation.Intents;
using CryBits.Simulation.Spatial;
using CommonMovement = CryBits.Definitions.Common.Movement;

namespace CryBits.Simulation.Systems.Movement;

public sealed class MovementSystem : ISimulationSystem
{
    public void Execute(World world, Tick tick)
    {
        foreach (var intent in tick.Intents.All)
        {
            if (intent is MoveIntent move)
            {
                ChangeDirection(world, move.SourceEntityId, move.Direction);
                Move(world, tick, move.SourceEntityId, move.Movement);
            }
        }
    }

    private void ChangeDirection(World world, EntityId entityId, Direction direction)
    {
        if (!world.IsAlive(entityId)) return;

        if (direction is < Direction.Up or > Direction.Right) return;
        if (world.Has<MapLoadingTag>(entityId)) return;

        world.Update<Position>(entityId, pos => pos with { Direction = direction });
    }

    private void Move(World world, Tick tick, EntityId entityId, CommonMovement movement)
    {
        if (!world.IsAlive(entityId)) return;
        var pos = world.Get<Position>(entityId)!;

        var dir = pos.Direction;
        var nextX = dir == Direction.Right ? pos.X + 1 : dir == Direction.Left ? pos.X - 1 : pos.X;
        var nextY = dir == Direction.Down ? pos.Y + 1 : dir == Direction.Up ? pos.Y - 1 : pos.Y;

        if (movement is < CommonMovement.Walking or > CommonMovement.Moving) return;
        if (world.Has<MapLoadingTag>(entityId)) return;

        if (ChunkGrid.IsTileBlocked(world, pos.MapId, nextX, nextY))
            return;

        if (ChunkGrid.FindAt<CollidableTag>(world, pos.MapId, nextX, nextY).HasValue)
            return;

        world.Update<Position>(entityId, p => p with { X = nextX, Y = nextY, Direction = dir });

        if (world.Has<PlayerTag>(entityId))
            tick.Events.Emit(new PlayerStartedMovingEvent(tick.TickNumber, entityId));
    }
}
