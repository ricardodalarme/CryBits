namespace CryBits.Client.Components.Combat;

internal struct DamageTintComponent
{
    public bool IsHurt;

    /// <summary>
    /// Seconds remaining on the hurt tint. Written as 0.325f on hit;
    /// decremented by dt each frame by <see cref="Systems.Combat.DamageTintSystem"/>.
    /// <see cref="IsHurt"/> is cleared automatically when this reaches zero.
    /// </summary>
    public float TimeRemaining;
}
