namespace CryBits.Client.ECS.Components;

/// <summary>
/// Party membership for the local player.
/// Stores the entity ids of current party members so the UI can look up
/// their <see cref="VitalsComponent"/> and <see cref="PlayerDataComponent"/>
/// without coupling to any player-class type.
/// Only present on the entity with <see cref="LocalPlayerTag"/>.
/// </summary>
public sealed class PartyComponent : IComponent
{
    /// <summary>Entity ids of the active party members (excluding the local player).</summary>
    public int[] MemberEntityIds { get; set; } = [];
}
