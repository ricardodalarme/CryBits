using CryBits.Client.Components;
using CryBits.Client.Framework.Assets;
using CryBits.Definitions.Maps;
using CryBits.Simulation.Core;
using SFML.Graphics;
using SFML.System;
using static CryBits.Definitions.Globals;
using Color = SFML.Graphics.Color;

namespace CryBits.Client.Rendering.Effects;

internal sealed class WeatherParticleRenderer(World world, SpriteBatch spriteBatch) : IRenderer
{
    private readonly VertexArray _batch = new(PrimitiveType.Triangles);

    private static readonly Color _particleTint = new(255, 255, 255, 150);

    public void Render()
    {
        _batch.Clear();

        foreach (var entityId in world.All)
        {
            var particle = world.Get<WeatherParticleComponent>(entityId);
            var transform = world.Get<TransformComponent>(entityId);
            if (particle == null || transform == null) continue;

            var srcX = particle.Type == WeatherType.Snow ? 32f : 0f;
            AppendQuad(_batch, transform.X, transform.Y, srcX, 0f, 32f, 32f, _particleTint);
        }

        if (_batch.VertexCount > 0)
            spriteBatch.RenderWindow.Draw(_batch, new RenderStates(Textures.Weather));

        foreach (var entityId in world.All)
        {
            var lightning = world.Get<LightningComponent>(entityId);
            if (lightning == null) continue;

            if (lightning.Intensity > 0)
                spriteBatch.Draw(
                    Textures.Blank,
                    0, 0, 0, 0, ScreenWidth, ScreenHeight,
                    new Color(255, 255, 255, lightning.Intensity));
        }
    }

    private static void AppendQuad(VertexArray va,
        float px, float py,
        float srcX, float srcY,
        float w, float h,
        Color tint)
    {
        va.Append(new Vertex(new Vector2f(px, py), tint, new Vector2f(srcX, srcY)));
        va.Append(new Vertex(new Vector2f(px + w, py), tint, new Vector2f(srcX + w, srcY)));
        va.Append(new Vertex(new Vector2f(px, py + h), tint, new Vector2f(srcX, srcY + h)));

        va.Append(new Vertex(new Vector2f(px, py + h), tint, new Vector2f(srcX, srcY + h)));
        va.Append(new Vertex(new Vector2f(px + w, py), tint, new Vector2f(srcX + w, srcY)));
        va.Append(new Vertex(new Vector2f(px + w, py + h), tint, new Vector2f(srcX + w, srcY + h)));
    }
}
