namespace CryBits.Client.Components.Movement;

/// <summary>
/// Marker component that identifies an entity as a solid body that blocks tile movement.
/// Query alongside <see cref="MovementComponent"/> and <see cref="MapIdComponent"/> to find all
/// entities that occupy a given tile on a given map without coupling to NPC or player tag types.
/// </summary>
internal struct CollisionComponent;
