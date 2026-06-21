using CryBits.Client.Components;
using CryBits.Client.Core;
using CryBits.Client.Worlds;
using CryBits.Definitions.Maps;
using CryBits.Simulation.State;
using static CryBits.Definitions.Globals;

namespace CryBits.Client.Systems.Map;

internal sealed class WeatherSimulationSystem(GameContext context) : IClientSystem
{
    private readonly List<EntityId> _pendingDestroy = [];

    private const float SnowDriftInterval = 0.035f;

    private float _snowMoveAccumulator;

    private Weather _lastWeatherType = Weather.Normal;

    public void Update(float dt)
    {
        var weatherData = context.CurrentMap?.Data.Weather;
        if (weatherData == null || weatherData.Type == Weather.Normal) return;

        var type = weatherData.Type;

        if (type != _lastWeatherType)
        {
            _snowMoveAccumulator = 0f;
            _lastWeatherType = type;
        }

        _snowMoveAccumulator += dt;
        bool snowMove = _snowMoveAccumulator >= SnowDriftInterval;
        if (snowMove) _snowMoveAccumulator -= SnowDriftInterval;

        _pendingDestroy.Clear();

        foreach (var state in context.World.All)
        {
            var particle = state.Get<WeatherParticleComponent>();
            var transform = state.Get<TransformComponent>();
            if (particle == null || transform == null) continue;

            switch (type)
            {
                case Weather.Raining or Weather.Thundering:
                    transform.X += particle.Speed;
                    transform.Y += particle.Speed;
                    break;

                case Weather.Snowing:
                    MoveSnow(particle, transform, snowMove);
                    break;
            }

            if (transform.X > ScreenWidth || transform.Y > ScreenHeight)
                _pendingDestroy.Add(state.Id);
        }

        foreach (var id in _pendingDestroy)
            context.World.Destroy(id);
    }

    private static void MoveSnow(WeatherParticleComponent p, TransformComponent t, bool xAxis)
    {
        var difference = Random.Shared.Next(0, SnowMovement / 3);
        var x1 = p.Start + SnowMovement + difference;
        var x2 = p.Start - SnowMovement - difference;

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
