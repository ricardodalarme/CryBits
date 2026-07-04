using SFML.Graphics;

namespace CryBits.Client.Components;

public sealed record SpriteComponent(Texture Texture, System.Drawing.Rectangle? SourceRect, Color Tint);
