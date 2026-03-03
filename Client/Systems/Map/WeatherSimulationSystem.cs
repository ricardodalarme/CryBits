using System;
using System.Collections.Generic;
using Arch.Core;
using Arch.System;
using CryBits.Client.Components.Core;
using CryBits.Client.Components.Map;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Worlds;
using CryBits.Enums;
using static CryBits.Globals;
using static CryBits.Utils;
using ArchEntity = Arch.Core.Entity;

namespace CryBits.Client.Systems.Map;

/// <summary>
/// Advances all weather particle simulations and manages the lightning flash each frame.
/// </summary>
internal sealed class WeatherSimulationSystem(World world, GameContext context) : BaseSystem<World, float>(world)
{
    private readonly QueryDescription _particleQuery =
        new QueryDescription().WithAll<WeatherParticleComponent, TransformComponent>();

    private readonly QueryDescription _lightningQuery =
        new QueryDescription().WithAll<LightningComponent>();

    // Reused each frame to avoid heap allocation while collecting off-screen entities.
    private readonly List<ArchEntity> _toDestroy = [];

    // Accumulator for snow horizontal drift cadence (fires every 35 ms).
    private float _snowAccumulator;

    private static readonly string[] _thunderList = [Sounds.Thunder1, Sounds.Thunder2, Sounds.Thunder3, Sounds.Thunder4];

    public override void Update(in float dt)
    {
        var weatherData = context.CurrentMap?.Data.Weather;
        if (weatherData == null || weatherData.Type == Weather.Normal) return;

        var type = weatherData.Type;
        var deltaTime = dt;

        // ── 1. Snow movement timer ───────────────────────────────────────────
        _snowAccumulator += deltaTime;
        bool snowMove = _snowAccumulator >= 0.035f;
        if (snowMove) _snowAccumulator = 0f;

        // ── 2. Lightning decay ───────────────────────────────────────────────
        World.Query(in _lightningQuery, (ref LightningComponent lightning) =>
        {
            if (lightning.Intensity > 0f)
                lightning.Intensity = MathF.Max(0f, lightning.Intensity - 400f * deltaTime);
        });

        // ── 3. Move particles; collect those that left the screen ────────────
        _toDestroy.Clear();
        int activeCount = 0;

        World.Query(in _particleQuery,
            (ArchEntity entity, ref WeatherParticleComponent particle, ref TransformComponent transform) =>
            {
                switch (type)
                {
                    case Weather.Raining or Weather.Thundering:
                        transform.X += particle.Speed;
                        transform.Y += particle.Speed;
                        break;

                    case Weather.Snowing:
                        MoveSnow(ref particle, ref transform, snowMove);
                        break;
                }

                if (transform.X > ScreenWidth || transform.Y > ScreenHeight)
                    _toDestroy.Add(entity);
                else
                    activeCount++;
            });

        // ── 4. Destroy off-screen particles ──────────────────────────────────
        foreach (var e in _toDestroy)
            World.Destroy(e);

        // ── 5. Spawn one new particle if under the limit and random check passes
        int maxParticles = type == Weather.Snowing ? MaxSnowParticles : MaxRainParticles;
        if (activeCount < maxParticles &&
            MyRandom.Next(0, MaxWeatherIntensity - weatherData.Intensity) == 0)
            SpawnParticle(type);

        // ── 6. Thunderstorm: random thunder sound and lightning trigger ───────
        if (type == Weather.Thundering)
            TryThunder(weatherData.Intensity);
    }

    // ────────────────────────────────────────────────────────────────────────────
    // Helpers
    // ────────────────────────────────────────────────────────────────────────────

    /// <summary>Creates a new particle entity initialised for the given weather type.</summary>
    private void SpawnParticle(Weather type)
    {
        int x, y;
        var particle = new WeatherParticleComponent { Type = type };

        switch (type)
        {
            case Weather.Raining or Weather.Thundering:
                particle.Speed = MyRandom.Next(8, 13);
                if (MyRandom.Next(2) == 0)
                {
                    x = -32;
                    y = MyRandom.Next(-32, ScreenHeight);
                }
                else
                {
                    x = MyRandom.Next(-32, ScreenWidth);
                    y = -32;
                }
                break;

            case Weather.Snowing:
                particle.Speed = MyRandom.Next(1, 3);
                particle.Start = MyRandom.Next(-32, ScreenWidth);
                particle.Back = MyRandom.Next(2) != 0;
                x = particle.Start;
                y = -32;
                break;

            default:
                return;
        }

        World.Create(new TransformComponent(x, y), particle);
    }

    /// <summary>
    /// Advances snow particle position: vertical drop each frame, horizontal
    /// oscillation every 35 ms (<paramref name="xAxis"/> gate).
    /// </summary>
    private static void MoveSnow(ref WeatherParticleComponent p, ref TransformComponent t, bool xAxis)
    {
        var difference = MyRandom.Next(0, SnowMovement / 3);
        var x1 = p.Start + SnowMovement + difference;
        var x2 = p.Start - SnowMovement - difference;

        // Reverse horizontal direction at oscillation limits.
        if (x1 <= t.X) p.Back = true;
        else if (x2 >= t.X) p.Back = false;

        t.Y += p.Speed;

        if (xAxis)
        {
            if (p.Back) t.X--;
            else t.X++;
        }
    }

    /// <summary>
    /// Randomly triggers a thunder sound and sets the lightning flash intensity.
    /// Called only for <see cref="Weather.Thundering"/> maps.
    /// </summary>
    private void TryThunder(byte intensity)
    {
        if (MyRandom.Next(0, MaxWeatherIntensity * 10 - intensity * 2) != 0) return;

        var thunder = MyRandom.Next(0, _thunderList.Length);
        AudioManager.Instance.PlaySound(_thunderList[thunder]);

        if (thunder < 3)
        {
            World.Query(in _lightningQuery, (ref LightningComponent lightning) =>
            {
                lightning.Intensity = 190f;
            });
        }
    }
}
