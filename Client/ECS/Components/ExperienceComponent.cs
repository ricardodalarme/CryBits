namespace CryBits.Client.ECS.Components;

/// <summary>
/// Experience and level-up state for the local player.
/// Only present on the entity with <see cref="LocalPlayerTag"/>.
/// </summary>
public sealed class ExperienceComponent : IComponent
{
    public int Current { get; set; }
    public int Needed { get; set; }

    /// <summary>Unspent stat points available for distribution.</summary>
    public short Points { get; set; }
}
