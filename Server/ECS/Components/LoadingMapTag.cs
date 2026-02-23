namespace CryBits.Server.ECS.Components;

/// <summary>
/// Marker tag present on a player entity while it is loading a new map.
/// Add this tag when <c>GettingMap = true</c>; remove it when done.
/// Presence is equivalent to the old <c>Player.GettingMap</c> bool.
/// </summary>
internal sealed class LoadingMapTag : ECS.IComponent;
