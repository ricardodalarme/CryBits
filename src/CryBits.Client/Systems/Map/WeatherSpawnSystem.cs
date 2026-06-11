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
/// Manages weather particle population: counts live particles and conditionally
/// spawns one new particle per frame based on map weather intensity.
/// </summary>
internal sealed class WeatherSpawnSystem(GameContext context) : BaseSystem<World, float>(context.World)
{
    private readonly QueryDescription _particleQuery =
        new QueryDescription().WithAll<WeatherParticleComponent>();

    public override void Update(in float dt)
    {
        var weatherData = context.CurrentMap?.Data.Weather;
        if (weatherData == null || weatherData.Type == Weather.Normal) return;

        var type = weatherData.Type;

        // Live count is accurate because WeatherSimulationSystem has already
        // played back its CommandBuffer, removing off-screen entities.
        var activeCount = 0;
        World.Query(in _particleQuery, _ => activeCount++);

        var maxParticles = type == Weather.Snowing ? MaxSnowParticles : MaxRainParticles;
        if (activeCount >= maxParticles) return;
        if (MyRandom.Next(0, MaxWeatherIntensity - weatherData.Intensity) != 0) return;

        SpawnParticle(type);
    }

    /// <summary>Creates a single new particle entity for the given weather type.</summary>
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
}