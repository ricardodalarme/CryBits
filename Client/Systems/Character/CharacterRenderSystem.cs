using System.Collections.Generic;
using System.Drawing;
using Arch.Core;
using Arch.System;
using CryBits.Client.Components.Character;
using CryBits.Client.Components.Core;
using CryBits.Client.Framework.Graphics;
using CryBits.Client.Graphics;

namespace CryBits.Client.Systems.Character;

/// <summary>
/// Renders all character entities (players and NPCs) sorted by world-Y each frame,
/// giving the correct top-down depth illusion without a true Z-buffer.
///
/// Draw order per character (back-to-front):
///   1. Shadow — an oval beneath the sprite, aligned to the character's feet.
///   2. Animated sprite — the current animation frame with damage tint applied.
///
/// This system replaces the legacy <c>PlayerRenderer</c> and <c>NpcRenderer</c> shadow
/// calls, and takes over character sprite rendering from the generic
/// <c>SpriteRenderSystem</c> (which is updated to exclude entities with
/// <see cref="ShadowComponent"/>).
/// </summary>
internal sealed class CharacterRenderSystem(World world, Renderer renderer) : BaseSystem<World, int>(world)
{
    /// <summary>
    /// Targets only character entities: must have all of the spatial/visual components
    /// plus the shadow marker that distinguishes them from other sprite entities.
    /// </summary>
    private readonly QueryDescription _query = new QueryDescription()
        .WithAll<TransformComponent, SpriteComponent, AnimatedSpriteComponent, ShadowComponent>();

    // Reused every frame — avoids per-frame heap allocation from Y-sort.
    private readonly List<(int Y, Entity Entity)> _drawList = new();

    public override void Update(in int t)
    {
        _drawList.Clear();

        // Collect all character entities with their Y coordinate for sorting.
        World.Query(in _query, (Entity entity, ref TransformComponent transform) =>
        {
            _drawList.Add((transform.Y, entity));
        });

        // Sort ascending by Y so characters lower on screen (higher Y) appear in front.
        _drawList.Sort(static (a, b) => a.Y.CompareTo(b.Y));

        foreach (var (_, entity) in _drawList)
        {
            ref var transform = ref World.Get<TransformComponent>(entity);
            ref var sprite = ref World.Get<SpriteComponent>(entity);
            ref var anim = ref World.Get<AnimatedSpriteComponent>(entity);

            DrawShadow(ref transform, ref anim);
            DrawSprite(ref transform, ref sprite, ref anim);
        }
    }

    /// <summary>
    /// Draws the shadow texture stretched to the sprite frame width and positioned at the
    /// character's feet, preserving the same visual offset as the legacy renderer.
    /// </summary>
    private void DrawShadow(
        ref TransformComponent transform,
        ref AnimatedSpriteComponent anim)
    {
        var texture = Textures.Shadow;
        var shadowSize = texture.ToSize();
        var source = new Rectangle(0, 0, shadowSize.Width, shadowSize.Height);

        // Align shadow to the bottom of the sprite frame, shifted up by the shadow height.
        // The +5 keeps it visually inside the character's feet rather than floating below.
        var dest = new Rectangle(
            transform.X,
            transform.Y + anim.FrameHeight - shadowSize.Height + 5,
            anim.FrameWidth,
            shadowSize.Height);

        renderer.Draw(texture, source, dest);
    }

    /// <summary>
    /// Draws the animated sprite frame using the source rect already computed by
    /// <c>AnimatedSpriteSystem</c> and the tint set by <c>DamageTintSystem</c>.
    /// </summary>
    private void DrawSprite(
        ref TransformComponent transform,
        ref SpriteComponent sprite,
        ref AnimatedSpriteComponent anim)
    {
        var source = new Rectangle(
            anim.CurrentFrameX * anim.FrameWidth,
            anim.CurrentFrameY * anim.FrameHeight,
            anim.FrameWidth,
            anim.FrameHeight);

        var dest = source with { X = transform.X, Y = transform.Y };
        renderer.Draw(sprite.Texture, source, dest, sprite.Tint);
    }
}
