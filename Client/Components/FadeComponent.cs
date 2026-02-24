namespace CryBits.Client.Components;

/// <summary>
/// Causes an entity to lose opacity over time and destroy itself when invisible.
/// </summary>
internal struct FadeComponent(float intervalSeconds, byte amountPerTick)
{
    public float Timer;
    public float IntervalSeconds = intervalSeconds;
    public byte AmountPerTick = amountPerTick;
}
