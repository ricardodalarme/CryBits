using CryBits.Client.Components.Core;
using SFML.Graphics;

namespace CryBits.Client.Components.Character;

/// <summary>
/// Marks an entity as having a ground shadow drawn beneath its sprite.
/// The vertical offset is computed at render time from <see cref="AnimatedSpriteComponent"/>
/// so the shadow aligns with the character's feet regardless of frame size.
/// </summary>
internal struct ShadowComponent(Texture texture)
{
    /// <summary>Shadow texture to render. Stretched horizontally to match the sprite frame width.</summary>
    public Texture Texture = texture;
}
