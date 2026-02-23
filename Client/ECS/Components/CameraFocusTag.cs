namespace CryBits.Client.ECS.Components;

/// <summary>
/// Marker component — the camera is centered on the entity carrying this tag.
/// Currently always the local player, but extracted as a tag so the camera system
/// is decoupled from any notion of "player".
/// </summary>
public sealed class CameraFocusTag : IComponent;
