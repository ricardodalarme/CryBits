namespace CryBits.Client.Components;

public sealed record AttackComponent(float AttackCountdown = 0f)
{
    public bool IsAttacking => AttackCountdown > 0f;
}
