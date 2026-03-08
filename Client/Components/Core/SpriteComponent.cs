using SFML.Graphics;
using Color = SFML.Graphics.Color;

namespace CryBits.Client.Components.Core;

/// <summary>
/// Describes how to draw an entity
/// </summary>
internal struct SpriteComponent(Texture texture, IntRect? sourceRect = null, Color? tint = null)
{
    public Texture Texture = texture;
    public IntRect? SourceRect = sourceRect;
    public Color Tint = tint ?? Color.White;
}
