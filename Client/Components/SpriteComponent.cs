using System.Drawing;
using SFML.Graphics;
using Color = SFML.Graphics.Color;

namespace CryBits.Client.Components;

/// <summary>
/// Describes how to draw an entity
/// </summary>
internal struct SpriteComponent(Texture texture, Rectangle? sourceRect = null, Color? tint = null)
{
    public Texture Texture = texture;
    public Rectangle? SourceRect = sourceRect;
    public Color Tint = tint ?? Color.White;
}
