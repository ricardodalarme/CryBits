using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CryBits.Client.Components;

public sealed record SpriteComponent(
    Texture2D Texture,
    Rectangle? SourceRect,
    Color Tint);