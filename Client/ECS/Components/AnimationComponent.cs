namespace CryBits.Client.ECS.Components;

/// <summary>
/// Drives the sprite sheet animation column for characters.
/// <c>Frame</c> toggles between left/right walking columns; <c>IsAttacking</c>
/// flips to the attack column for the duration of the attack window.
/// </summary>
public sealed class AnimationComponent : IComponent
{
    /// <summary>Current animation column (maps to <c>AnimationRight/Left/Attack</c> constants).</summary>
    public byte Frame { get; set; } = Globals.AnimationRight;

    public bool IsAttacking { get; set; }

    /// <summary><c>Environment.TickCount</c> at which the attack began (0 = not attacking).</summary>
    public int AttackTimer { get; set; }
}
