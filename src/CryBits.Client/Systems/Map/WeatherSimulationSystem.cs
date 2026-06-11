using Arch.Buffer;
using Arch.Core;
using Arch.System;
using CryBits.Client.Components.Core;
using CryBits.Client.Components.Map;
using CryBits.Client.Worlds;
using CryBits.Enums;
using static CryBits.Globals;
using static CryBits.Utils.RandomUtils;

namespace CryBits.Client.Systems.Map;

/// <summary>
/// Moves every active weather particle and culls any that have left the screen.
/// </summary>
internal sealed class WeatherSimulationSystem(GameContext context) : BaseSystem<World, float>(context.World)
{
    private readonly QueryDescription _particleQuery =
        new QueryDescription().WithAll<WeatherParticleComponent, TransformComponent>();

    // Defers entity destruction so the world structure is never mutated mid-query.
    private readonly CommandBuffer _commandBuffer = new();

    /// <summary>Seconds between snow horizontal drift steps (35 ms).</summary>
    private const float SnowDriftInterval = 0.035f;

    private float _snowMoveAccumulator;

    // Tracks the last known weather type to detect transitions and reset the accumulator.
    private Weather _lastWeatherType = Weather.Normal;

    public override void Update(in float dt)
    {
        var weatherData = context.CurrentMap?.Data.Weather;
        if (weatherData == null || weatherData.Type == Weather.Normal) return;

        var type = weatherData.Type;

        // Reset the snow accumulator on weather-type transitions so the first
        // drift tick after switching to snow starts from a clean baseline.
        if (type != _lastWeatherType)
        {
            _snowMoveAccumulator = 0f;
            _lastWeatherType = type;
        }

        // ── Snow movement timer ───────────────────────────────────────────────
        _snowMoveAccumulator += dt;
        bool snowMove = _snowMoveAccumulator >= SnowDriftInterval;
        if (snowMove) _snowMoveAccumulator -= SnowDriftInterval;

        // ── Move particles; queue off-screen ones for destruction ─────────────
        World.Query(in _particleQuery,
            (Entity entity, ref WeatherParticleComponent particle, ref TransformComponent transform) =>
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
                    _commandBuffer.Destroy(in entity);
            });

        _commandBuffer.Playback(World);
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
}
