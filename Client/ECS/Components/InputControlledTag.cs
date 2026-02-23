namespace CryBits.Client.ECS.Components;

/// <summary>
/// Marker component — entity that receives keyboard/mouse input from the player.
/// Systems like <see cref="Systems.PlayerInputSystem"/> only process entities that carry this tag.
/// </summary>
public sealed class InputControlledTag : IComponent;
