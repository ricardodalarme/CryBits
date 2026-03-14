using CryBits.Enums;

namespace CryBits.Client.Components.Movement;

internal struct CharacterStateComponent
{
    public Direction Direction;
    public bool IsMoving;
    public bool IsAttacking;
    /// <summary>
    /// Remaining seconds of the current attack animation. Set to <c>AttackSpeed / 1000f</c>
    /// when an attack begins; counted down by dt in
    /// <see cref="Systems.Movement.CharacterAnimationControllerSystem"/>;
    /// cleared to 0 and <see cref="IsAttacking"/> reset when elapsed.
    /// </summary>
    public float AttackCountdown;
}
