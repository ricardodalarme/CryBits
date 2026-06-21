using CryBits.Client.Components;
using CryBits.Client.Core;
using CryBits.Client.Worlds;
using CryBits.Definitions.Maps;
using static CryBits.Definitions.Globals;

namespace CryBits.Client.Systems.Map;

internal sealed class WeatherSpawnSystem(GameContext context) : IClientSystem
{
    public void Update(float dt)
    {
        var weatherData = context.CurrentMap?.Data.Weather;
        if (weatherData == null || weatherData.Type == Weather.Normal) return;

        var type = weatherData.Type;

        var activeCount = 0;
        foreach (var _ in context.World.All)
        {
            if (context.World.Get<WeatherParticleComponent>(_.Id) != null)
                activeCount++;
        }

        var maxParticles = type == Weather.Snowing ? MaxSnowParticles : MaxRainParticles;
        if (activeCount >= maxParticles) return;
        if (Random.Shared.Next(0, MaxWeatherIntensity - weatherData.Intensity) != 0) return;

        SpawnParticle(type);
    }

    private void SpawnParticle(Weather type)
    {
        int x, y;
        var particle = new WeatherParticleComponent { Type = type };

        switch (type)
        {
            case Weather.Raining or Weather.Thundering:
                particle.Speed = Random.Shared.Next(8, 13);
                if (Random.Shared.Next(2) == 0)
                {
                    x = -32;
                    y = Random.Shared.Next(-32, ScreenHeight);
                }
                else
                {
                    x = Random.Shared.Next(-32, ScreenWidth);
                    y = -32;
                }
                break;

            case Weather.Snowing:
                particle.Speed = Random.Shared.Next(1, 3);
                particle.Start = Random.Shared.Next(-32, ScreenWidth);
                particle.Back = Random.Shared.Next(2) != 0;
                x = particle.Start;
                y = -32;
                break;

            default:
                return;
        }

        var id = context.World.Spawn();
        context.World.Set(id, new TransformComponent { X = x, Y = y });
        context.World.Set(id, particle);
    }
}
