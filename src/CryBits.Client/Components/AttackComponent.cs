namespace CryBits.Client.Components;

public sealed class AttackComponent
{
    public float AttackCountdown { get; set; }

    public bool IsAttacking => AttackCountdown > 0f;
}
