namespace CryBits.Client.Components.Movement;

internal struct CharacterStateComponent
{
    public bool IsAttacking;

    /// <summary>
    /// Seconds remaining in the current attack cooldown.
    /// Written as <c>AttackSpeed / 1000f</c> when an attack starts;
    /// ticked down by dt in <see cref="Systems.Movement.CharacterAnimationControllerSystem"/>.
    /// <see cref="IsAttacking"/> is cleared when this reaches zero.
    /// </summary>
    public float AttackTimer;
}
