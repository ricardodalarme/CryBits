using CryBits.Enums;

namespace CryBits.Client.Components;

internal struct CharacterStateComponent
{
    public Direction Direction;
    public bool IsMoving;
    public bool IsAttacking;
    public int AttackTimer;
}
