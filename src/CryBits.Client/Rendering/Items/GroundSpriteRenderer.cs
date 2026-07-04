using CryBits.Client.Components;
using CryBits.Client.Framework.Assets;
using CryBits.Simulation.Core;
using System.Drawing;

namespace CryBits.Client.Rendering.Items;

internal sealed class GroundSpriteRenderer(World world, SpriteBatch spriteBatch) : IRenderer
{
    public void Render()
    {
        foreach (var state in world.All)
        {
            var transform = state.Get<TransformComponent>();
            var sprite = state.Get<SpriteComponent>();
            if (transform == null || sprite == null) continue;
            if (state.Has<AnimationState>()) continue;

            var source = sprite.SourceRect.HasValue ? sprite.SourceRect.Value : new Rectangle(Point.Empty, sprite.Texture.ToSize());
            var dest = source with { X = transform.X, Y = transform.Y };

            spriteBatch.Draw(sprite.Texture, source, dest, sprite.Tint);
        }
    }
}
