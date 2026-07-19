using CryBits.Client.Components;
using CryBits.Client.Core;
using CryBits.Definitions.Maps;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Spatial;
using static CryBits.Definitions.Globals;
using MapDef = CryBits.Definitions.Maps.Map;

namespace CryBits.Client.Systems.Map;

internal sealed class WeatherSimulationSystem(GameContext context) : IClientSystem
{
    private const float SnowDriftInterval = 0.035f;

    private float _snowMoveAccumulator;

    private WeatherType _lastWeatherType = WeatherType.None;

    private WeatherType GetEffectiveWeather(MapDef map)
    {
        var playerId = context.LocalPlayerEntity;
        if (playerId == null) return map.DefaultWeather;
        var pos = context.World.Get<Position>(playerId.Value);
        if (pos == null) return map.DefaultWeather;
        var chunkCoord = ChunkGrid.FromPosition(pos.X, pos.Y);
        if (map.Chunks.TryGetValue(chunkCoord, out var chunk) && chunk.WeatherOverride.HasValue)
            return chunk.WeatherOverride.Value;
        return map.DefaultWeather;
    }

    public void Update(float dt)
    {
        var map = context.CurrentMap;
        if (map == null) return;

        var type = GetEffectiveWeather(map);
        if (type == WeatherType.None) return;

        if (type != _lastWeatherType)
        {
            _snowMoveAccumulator = 0f;
            _lastWeatherType = type;
        }

        _snowMoveAccumulator += dt;
        var snowMove = _snowMoveAccumulator >= SnowDriftInterval;
        if (snowMove) _snowMoveAccumulator -= SnowDriftInterval;

        var commands = new CommandBuffer(context.World);

        foreach (var entityId in context.World.All)
        {
            var particle = context.World.Get<WeatherParticleComponent>(entityId);
            var transform = context.World.Get<TransformComponent>(entityId);
            if (particle == null || transform == null) continue;

            switch (type)
            {
                case WeatherType.Rain or WeatherType.Thunder:
                    context.World.Set(entityId, new TransformComponent(
                        transform.X + particle.Speed,
                        transform.Y + particle.Speed
                    ));
                    break;

                case WeatherType.Snow:
                    MoveSnow(context.World, entityId, particle, transform, snowMove);
                    break;
            }

            var newTransform = context.World.Get<TransformComponent>(entityId) ?? transform;
            if (newTransform.X > ScreenWidth || newTransform.Y > ScreenHeight)
                commands.Destroy(entityId);
        }

        commands.Flush();
    }

    private static void MoveSnow(World world, EntityId entityId, WeatherParticleComponent p, TransformComponent t, bool xAxis)
    {
        var difference = Random.Shared.Next(0, SnowMovement / 3);
        var x1 = p.Start + SnowMovement + difference;
        var x2 = p.Start - SnowMovement - difference;

        var newBack = x1 <= t.X ? true : x2 >= t.X ? false : p.Back;
        var newY = t.Y + p.Speed;
        var newX = xAxis ? (newBack ? t.X - 1 : t.X + 1) : t.X;

        world.Set(entityId, p with { Back = newBack });
        world.Set(entityId, new TransformComponent(newX, newY));
    }
}
