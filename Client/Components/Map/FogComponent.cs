namespace CryBits.Client.Components.Map;

/// <summary>
/// Drives the seamless scrolling fog overlay for the current map.
/// A single entity carrying this component is created by <see cref="Spawners.FogSpawner"/>
/// on map load and destroyed when the map changes or has no fog.
///
/// The SFML texture on the accompanying <c>SpriteComponent</c> must have
/// <c>Repeated = true</c> (set once at load time in <c>Textures</c>).
/// <see cref="Systems.Map.FogSystem"/> advances <see cref="OffsetX"/>/<see cref="OffsetY"/>
/// each frame; <see cref="Systems.Map.FogRenderSystem"/> reads them to build the source rect.
/// </summary>
internal struct FogComponent(float speedX, float speedY)
{
    /// <summary>Horizontal scroll speed in pixels per second. Negative = left.</summary>
    public float SpeedX = speedX;

    /// <summary>Vertical scroll speed in pixels per second. Negative = up.</summary>
    public float SpeedY = speedY;

    /// <summary>Accumulated horizontal scroll offset in pixels.</summary>
    public float OffsetX;

    /// <summary>Accumulated vertical scroll offset in pixels.</summary>
    public float OffsetY;
}
