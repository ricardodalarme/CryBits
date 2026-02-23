namespace CryBits.Client.ECS.Components;

/// <summary>
/// Visual identity of a character: which texture strip to use and whether
/// the entity is currently tinted red from receiving damage.
/// </summary>
public sealed class CharacterSpriteComponent : IComponent
{
    /// <summary>1-based index into <c>Textures.Characters</c>. 0 = no texture.</summary>
    public short TextureNum { get; set; }

    /// <summary>
    /// <c>Environment.TickCount</c> at which this entity was last hurt (0 = not hurt).
    /// The renderer applies a red tint while <c>TickCount &lt; HurtTimer + HurtDuration</c>.
    /// </summary>
    public int HurtTimer { get; set; }
}
