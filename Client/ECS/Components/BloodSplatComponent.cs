namespace CryBits.Client.ECS.Components;

/// <summary>
/// A transient blood-splatter decal on the map.
/// The <see cref="Systems.BloodFadeSystem"/> decrements <see cref="Opacity"/> every
/// 100 ms; when it reaches 0 the entity is destroyed.
/// </summary>
public sealed class BloodSplatComponent : IComponent
{
    /// <summary>Column index into the blood texture strip (0–2).</summary>
    public byte TextureNum { get; set; }

    public short TileX { get; set; }
    public short TileY { get; set; }

    /// <summary>Alpha value 0–255; starts at 255 and fades to 0.</summary>
    public byte Opacity { get; set; } = 255;

    /// <summary>Next tick at which opacity should be decremented.</summary>
    public int NextFadeAt { get; set; }
}
