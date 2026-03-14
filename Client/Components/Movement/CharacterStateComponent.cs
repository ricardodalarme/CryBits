using CryBits.Enums;

namespace CryBits.Client.Components.Movement;

internal struct CharacterStateComponent
{
    public Direction Direction;
    public bool IsMoving;
    public float AttackCountdown;

    public readonly bool IsAttacking => AttackCountdown > 0f;
}
