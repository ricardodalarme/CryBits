using CryBits.Client.Components;
using CryBits.Client.Core;
using CryBits.Client.Framework.Graphics;
using CryBits.Client.Graphics;
using CryBits.Definitions.Maps;
using CryBits.Simulation.Core;
using SFML.Graphics;
using SFML.System;
using static CryBits.Definitions.Globals;
using Color = SFML.Graphics.Color;

namespace CryBits.Client.Systems.Map;

internal sealed class WeatherRenderSystem(World world, Renderer renderer) : IClientRenderSystem
{
    private readonly VertexArray _batch = new(PrimitiveType.Triangles);

    private static readonly Color _particleTint = new(255, 255, 255, 150);

    public void Render()
    {
        _batch.Clear();

        foreach (var state in world.All)
        {
            var particle = state.Get<WeatherParticleComponent>();
            var transform = state.Get<TransformComponent>();
            if (particle == null || transform == null) continue;

            float srcX = particle.Type == WeatherType.Snow ? 32f : 0f;
            AppendQuad(_batch, transform.X, transform.Y, srcX, 0f, 32f, 32f, _particleTint);
        }

        if (_batch.VertexCount > 0)
            renderer.RenderWindow.Draw(_batch, new RenderStates(Textures.Weather));

        foreach (var state in world.All)
        {
            var lightning = state.Get<LightningComponent>();
            if (lightning == null) continue;

            if (lightning.Intensity > 0)
                renderer.Draw(
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
