namespace CryBits.Client.Components.Combat;

internal struct HurtComponent()
{
    /// <summary>Duration of the hurt tint in seconds.</summary>
    public const float Duration = 0.325f;

    public float HurtCountdown = Duration;
}
