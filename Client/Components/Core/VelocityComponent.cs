namespace CryBits.Client.Components.Core;

/// <summary>
/// Velocity of an entity in world-pixels per second.
/// </summary>
internal struct VelocityComponent(float x, float y)
{
    public float X = x;
    public float Y = y;
}
