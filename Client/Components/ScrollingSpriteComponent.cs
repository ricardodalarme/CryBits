namespace CryBits.Client.Components;

/// <summary>
/// Causes a <see cref="SpriteComponent"/>'s SourceRect to endlessly scroll,
/// producing seamless infinite panning. Requires the SFML Texture to have
/// <c>Repeated = true</c> (set once at load time).
/// </summary>
internal struct ScrollingSpriteComponent(float speedX, float speedY)
{
    /// <summary>Horizontal scroll speed in pixels per second. Negative = left.</summary>
    public float SpeedX = speedX;

    /// <summary>Vertical scroll speed in pixels per second. Negative = up.</summary>
    public float SpeedY = speedY;

    /// <summary>Accumulated horizontal offset</summary>
    public float ExactX = 0;

    /// <summary>Accumulated vertical offset.</summary>
    public float ExactY;
}
