namespace CryBits.Client.Components;

public sealed class HurtComponent
{
    public const float Duration = 0.325f;

    public float HurtCountdown { get; set; } = Duration;
}
