namespace CryBits.Client.Components;

public sealed record FadeComponent(float Timer = 0f, float IntervalSeconds = 0.1f, byte AmountPerTick = 1);
