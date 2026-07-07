using CryBits.Client.Components;
using CryBits.Client.Framework.Assets;
using CryBits.Simulation.Core;
using System.Drawing;

namespace CryBits.Client.Rendering.Items;

internal sealed class GroundSpriteRenderer(World world, SpriteBatch spriteBatch) : IRenderer
{
    public void Render()
    {
        foreach (var entityId in world.All)
        {
            var transform = world.Get<TransformComponent>(entityId);
            var sprite = world.Get<SpriteComponent>(entityId);
            if (transform == null || sprite == null) continue;
            if (world.Has<AnimationState>(entityId)) continue;

            var source = sprite.SourceRect.HasValue ? sprite.SourceRect.Value : new Rectangle(Point.Empty, sprite.Texture.ToSize());
            var dest = source with { X = transform.X, Y = transform.Y };

            spriteBatch.Draw(sprite.Texture, source, dest, sprite.Tint);
        }
    }
}
