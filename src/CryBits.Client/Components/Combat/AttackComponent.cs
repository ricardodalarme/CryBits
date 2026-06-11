namespace CryBits.Client.Components.Combat;

internal struct AttackComponent
{
    public float AttackCountdown;

    public readonly bool IsAttacking => AttackCountdown > 0f;
}
