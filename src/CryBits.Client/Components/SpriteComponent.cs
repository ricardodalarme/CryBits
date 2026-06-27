using SFML.Graphics;

namespace CryBits.Client.Components;

public sealed record class SpriteComponent(Texture Texture, System.Drawing.Rectangle? SourceRect, Color Tint);
