using Arch.Core;
using Arch.System;
using CryBits.Client.Components.Core;
using CryBits.Client.Components.Map;
using CryBits.Client.Framework.Graphics;
using CryBits.Client.Graphics;
using CryBits.Client.Worlds;
using CryBits.Enums;
using SFML.Graphics;
using SFML.System;
using static CryBits.Globals;
using Color = SFML.Graphics.Color;

namespace CryBits.Client.Systems.Map;

/// <summary>
/// Batched renderer for weather particles and the lightning full-screen flash.
/// </summary>
internal sealed class WeatherRenderSystem(World world, GameContext context, Renderer renderer) : BaseSystem<World, int>(world)
{
    private readonly QueryDescription _particleQuery =
        new QueryDescription().WithAll<WeatherParticleComponent, TransformComponent>();

    private readonly QueryDescription _lightningQuery =
        new QueryDescription().WithAll<LightningComponent>();

    // Single VertexArray reused each frame — avoids per-frame heap allocation.
    private readonly VertexArray _batch = new(PrimitiveType.Triangles);

    public override void Update(in int t)
    {
        var weatherData = context.CurrentMap?.Data.Weather;
        if (weatherData == null || weatherData.Type == Weather.Normal) return;

        // Snow uses a different horizontal source strip on the weather sprite sheet.
        float srcX = weatherData.Type == Weather.Snowing ? 32f : 0f;
        var tint = new Color(255, 255, 255, 150);

        // ── 1. Batch all visible particles ───────────────────────────────────
        _batch.Clear();
        World.Query(in _particleQuery, (ref WeatherParticleComponent _, ref TransformComponent transform) => AppendQuad(_batch, transform.X, transform.Y, srcX, 0f, 32f, 32f, tint));

        if (_batch.VertexCount > 0)
            renderer.RenderWindow.Draw(_batch, new RenderStates(Textures.Weather));

        // ── 2. Full-screen lightning overlay ─────────────────────────────────
        World.Query(in _lightningQuery, (ref LightningComponent lightning) =>
        {
            if (lightning.Intensity > 0)
                renderer.Draw(
                    Textures.Blank,
                    0, 0, 0, 0, ScreenWidth, ScreenHeight,
                    new Color(255, 255, 255, lightning.Intensity));
        });
    }

    // ────────────────────────────────────────────────────────────────────────────
    // Geometry helper — same two-triangle quad as MapRenderer.AppendQuad
    // ────────────────────────────────────────────────────────────────────────────

    private static void AppendQuad(VertexArray va,
        float px, float py,
        float srcX, float srcY,
        float w, float h,
        Color tint)
    {
        // Triangle 1
        va.Append(new Vertex(new Vector2f(px, py), tint, new Vector2f(srcX, srcY)));
        va.Append(new Vertex(new Vector2f(px + w, py), tint, new Vector2f(srcX + w, srcY)));
        va.Append(new Vertex(new Vector2f(px, py + h), tint, new Vector2f(srcX, srcY + h)));

        // Triangle 2
        va.Append(new Vertex(new Vector2f(px, py + h), tint, new Vector2f(srcX, srcY + h)));
        va.Append(new Vertex(new Vector2f(px + w, py), tint, new Vector2f(srcX + w, srcY)));
        va.Append(new Vertex(new Vector2f(px + w, py + h), tint, new Vector2f(srcX + w, srcY + h)));
    }
}
