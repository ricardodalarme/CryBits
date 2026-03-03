namespace CryBits.Client.Components.Combat;

internal struct DamageTintComponent
{
    public bool IsHurt;
    /// <summary>
    /// <see cref="System.Environment.TickCount"/> when the last hit was received.
    /// Used by <see cref="Systems.Combat.DamageTintSystem"/> to auto-clear
    /// <see cref="IsHurt"/> after 325 ms without any per-entity legacy field.
    /// </summary>
    public int HurtTimestamp;
}
