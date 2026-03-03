namespace CryBits.Client.Components.Character;

/// <summary>
/// Marker component that identifies an entity as a player character.
/// Use this in queries to target only players, e.g. for name colour rules or
/// player-specific gameplay logic, without coupling code to the entity collection.
/// </summary>
internal struct PlayerTagComponent;
