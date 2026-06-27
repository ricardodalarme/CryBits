namespace CryBits.Client.Components;

public sealed record class HurtComponent(float HurtCountdown = HurtComponent.Duration)
{
    public const float Duration = 0.325f;
}
