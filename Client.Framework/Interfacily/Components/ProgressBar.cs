namespace CryBits.Client.Framework.Interfacily.Components;

/// <summary>
/// A component that renders a horizontal fill bar from a sprite-sheet row.
/// The game view calls <see cref="SetValue"/> each frame; the renderer reads
/// <see cref="FillWidth"/> and stays data-blind (identical pattern to <see cref="Label"/>).
/// </summary>
public class ProgressBar : Component
{
    /// <summary>Y offset into the bar sprite sheet (selects the colour row).</summary>
    public int SourceY { get; set; }

    /// <summary>Maximum fill width in pixels (equals the sprite-sheet width).</summary>
    public int Width { get; set; }

    /// <summary>Fill height in pixels.</summary>
    public int Height { get; set; }

    private float _value;

    /// <summary>Sets the fill ratio (clamped 0–1).</summary>
    public void SetValue(float value) => _value = Math.Clamp(value, 0f, 1f);

    /// <summary>Pixel width to draw at the current fill level.</summary>
    public int FillWidth => (int)(Width * _value);

    public override string ToString() => "[ProgressBar] " + Name;
}
