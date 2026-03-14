namespace CryBits.Client.Components.Combat;

internal struct DamageTintComponent
{
    /// <summary>Duration of the hurt tint in seconds.</summary>
    public const float Duration = 0.325f;

    public bool IsHurt;
    public float HurtCountdown;
}
