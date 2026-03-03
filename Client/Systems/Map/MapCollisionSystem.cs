using Arch.Core;
using Arch.System;
using CryBits.Client.Components.Character;
using CryBits.Client.Components.Movement;
using CryBits.Client.Worlds;

namespace CryBits.Client.Systems.Map;

/// <summary>
/// Rebuilds <see cref="ClientMap.CollisionGrid"/> for the single active map every frame.
///
/// Execution order: must run <b>before</b> <c>LocalPlayerInputSystem</c> so
/// that <see cref="ClientMap.TileBlocked"/> always reads a fresh grid.
///
/// Design notes:
///   • Only one map is ever active at runtime (<c>GameContext.CurrentMap</c>).
///     Map data for other maps is cached solely for revision/disk purposes and
///     carries no live entities, so there is no need to maintain multiple grids.
///   • NPCs are always on the current map and do not carry a
///     <see cref="MapIdComponent"/>; they are captured by the NPC query.
///   • Players on other maps are destroyed during map-transition cleanup
///     (<c>MapRevision</c> packet), so the player query captures only the
///     entities that belong to the current map without any map-id filtering.
///
/// The result is an O(N) rebuild (N = NPCs + players on screen) that replaces
/// the previous per-frame O(N) scan inside every collision check.
/// Any subsequent <c>TileBlocked</c> call is now a single array read: O(1).
/// </summary>
internal sealed class MapCollisionSystem(World world, GameContext context)
    : BaseSystem<World, float>(world)
{
    private static readonly QueryDescription _npcQuery = new QueryDescription()
        .WithAll<NpcTagComponent, MovementComponent>();

    private static readonly QueryDescription _playerQuery = new QueryDescription()
        .WithAll<PlayerTagComponent, MovementComponent>();

    public override void Update(in float dt)
    {
        if (context.CurrentMap is null) return;

        var grid = context.CurrentMap.CollisionGrid;

        // 1. Reset prior frame's occupancy.
        grid.Clear();

        // 2. Stamp NPC positions — NPCs always belong to the current map.
        World.Query(in _npcQuery,
            (Entity entity, ref MovementComponent movement) =>
                grid.Set(movement.TileX, movement.TileY, entity));

        // 3. Stamp player positions — only players on the current map exist
        //    in the world after map-transition cleanup.
        World.Query(in _playerQuery,
            (Entity entity, ref MovementComponent movement) =>
                grid.Set(movement.TileX, movement.TileY, entity));
    }
}
