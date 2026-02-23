namespace CryBits.Client.ECS.Components;

/// <summary>
/// Marker component — the entity that carries this tag is the locally-controlled player.
/// There is always exactly one alive entity with this component.
/// Query via <c>World.FindSingle&lt;LocalPlayerTag&gt;()</c>.
/// </summary>
public sealed class LocalPlayerTag : IComponent;
