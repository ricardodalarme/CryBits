using CryBits.Client.Components;
using CryBits.Client.Framework.Assets;
using CryBits.Definitions.Maps;
using CryBits.Simulation.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static CryBits.Definitions.Globals;

namespace CryBits.Client.Rendering.Effects;

internal sealed class WeatherParticleRenderer(World world, SpriteBatch spriteBatch) : IRenderer
{
    private static readonly Color _particleTint = new(255, 255, 255, 150);

    public void Render()
    {
        foreach (var entityId in world.All)
        {
            var particle = world.Get<WeatherParticleComponent>(entityId);
            var transform = world.Get<TransformComponent>(entityId);
            if (particle == null || transform == null) continue;

            var srcX = particle.Type == WeatherType.Snow ? 32 : 0;
            spriteBatch.Draw(Textures.Weather,
                new Rectangle(transform.X, transform.Y, 32, 32),
                new Rectangle(srcX, 0, 32, 32),
                _particleTint);
        }

        foreach (var entityId in world.All)
        {
            var lightning = world.Get<LightningComponent>(entityId);
            if (lightning == null) continue;

            if (lightning.Intensity > 0)
                spriteBatch.Draw(Textures.Blank,
                    new Rectangle(0, 0, ScreenWidth, ScreenHeight),
                    new Rectangle(0, 0, ScreenWidth, ScreenHeight),
                    new Color(255, 255, 255, (int)lightning.Intensity));
        }
    }
}
