using System.Drawing;

namespace CryBits.Client.Framework.Interfacily.Components;

/// <summary>
/// A placeholder component that delegates all drawing to an external subscriber via <see cref="OnRender"/>.
/// <para>
/// The view binds <see cref="OnRender"/> to its rendering logic (e.g. drawing a character avatar).
/// The renderer calls <see cref="Render"/> each frame — it stays completely data-blind.
/// </para>
/// </summary>
public class Picture : Component
{
    /// <summary>Width of the picture area in pixels.</summary>
    public int Width { get; set; }

    /// <summary>Height of the picture area in pixels.</summary>
    public int Height { get; set; }

    /// <summary>Fires each render frame with the component's top-left position so the subscriber can draw content.</summary>
    public event Action<Point>? OnRender;

    /// <summary>Invokes <see cref="OnRender"/> if the component is visible.</summary>
    public void Render()
    {
        if (!Visible) return;
        OnRender?.Invoke(Position);
    }

    public override string ToString() => "[Picture] " + Name;
}
