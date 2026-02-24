using System.Drawing;
using SFML.Graphics;

namespace CryBits.Client.Components;

/// <summary>
/// Describes how to draw an entity
/// </summary>
internal struct SpriteComponent(Texture texture, Rectangle? sourceRect = null, byte opacity = 255)
{
    public Texture Texture = texture;
    public Rectangle? SourceRect = sourceRect;
    public byte Opacity = opacity;
}
