using CryBits.Enums;

namespace CryBits.Client.Components.Movement;

internal struct CharacterStateComponent
{
    public bool IsMoving;
    public float AttackCountdown;

    public readonly bool IsAttacking => AttackCountdown > 0f;
}
