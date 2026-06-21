using SFML.Graphics;

namespace CryBits.Client.Components;

public sealed class SpriteComponent
{
    public Texture Texture { get; set; } = null!;
    public System.Drawing.Rectangle? SourceRect { get; set; }
    public Color Tint { get; set; } = Color.White;
}
