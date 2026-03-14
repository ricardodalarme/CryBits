using SFML.Graphics;
using System.Drawing;
using Color = SFML.Graphics.Color;

namespace CryBits.Client.Components.Core;

/// <summary>
/// Describes how to draw an entity
/// </summary>
internal struct SpriteComponent(Texture texture, Rectangle? sourceRect = null, Color? tint = null)
{
    public Texture Texture = texture;
    public Rectangle? SourceRect = sourceRect;
    public Color Tint = tint ?? Color.White;
}
