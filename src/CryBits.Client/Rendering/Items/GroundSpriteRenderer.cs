using CryBits.Client.Components;
using Microsoft.Xna.Framework.Graphics;
using CryBits.Simulation.Core;
using Microsoft.Xna.Framework;

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

            var source = new Rectangle(0, 0, sprite.Texture.Width, sprite.Texture.Height);
            var dest = new Rectangle(transform.X, transform.Y, source.Width, source.Height);

            spriteBatch.Draw(sprite.Texture, source, dest, sprite.Tint);
        }
    }
}
