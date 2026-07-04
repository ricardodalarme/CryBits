using CryBits.Client.Components;
using CryBits.Client.Worlds;
using CryBits.Definitions.Maps;
using CryBits.Simulation.Components;
using CryBits.Simulation.Spatial;
using static CryBits.Definitions.Globals;
using MapDef = CryBits.Definitions.Maps.Map;

namespace CryBits.Client.Systems.Map;

internal sealed class WeatherSpawnSystem(GameContext context) : IClientSystem
{
    public void Update(float dt)
    {
        var map = context.CurrentMap;
        if (map == null) return;

        var type = GetEffectiveWeather(map);

        var activeCount = 0;
        foreach (var _ in context.World.All)
        {
            if (context.World.Get<WeatherParticleComponent>(_.Id) != null)
                activeCount++;
        }

        var maxParticles = type == WeatherType.Snow ? MaxSnowParticles : MaxRainParticles;
        if (activeCount >= maxParticles) return;
        if (Random.Shared.Next(0, 100) != 0) return;

        SpawnParticle(type);
    }

    private WeatherType GetEffectiveWeather(MapDef map)
    {
        var playerId = context.LocalPlayer.Entity;
        if (playerId == null) return map.DefaultWeather;
        var pos = context.World.Get<Position>(playerId.Value);
        if (pos == null) return map.DefaultWeather;
        var chunkCoord = ChunkGrid.FromPosition(pos.X, pos.Y);
        if (map.Chunks.TryGetValue(chunkCoord, out var chunk) && chunk.WeatherOverride.HasValue)
            return chunk.WeatherOverride.Value;
        return map.DefaultWeather;
    }

    private void SpawnParticle(WeatherType type)
    {
        int x, y, speed;
        int start = 0;
        bool back = false;

        switch (type)
        {
            case WeatherType.Rain or WeatherType.Thunder:
                speed = Random.Shared.Next(8, 13);
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

            case WeatherType.Snow:
                speed = Random.Shared.Next(1, 3);
                start = Random.Shared.Next(-32, ScreenWidth);
                back = Random.Shared.Next(2) != 0;
                x = start;
                y = -32;
                break;

            default:
                return;
        }

        var id = context.World.Spawn();
        context.World.Set(id, new TransformComponent(x, y));
        context.World.Set(id, new WeatherParticleComponent(speed, start, back, type));
    }
}
