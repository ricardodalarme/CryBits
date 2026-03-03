using CryBits.Client.Components.Core;

namespace CryBits.Client.Components.Character;

/// <summary>
/// Marks an entity as having a ground shadow drawn beneath its sprite.
/// The vertical offset is computed at render time from <see cref="AnimatedSpriteComponent"/>
/// so the shadow aligns with the character's feet regardless of frame size.
/// </summary>
internal struct ShadowComponent;