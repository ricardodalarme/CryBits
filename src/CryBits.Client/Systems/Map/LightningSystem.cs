using CryBits.Client.Components;
using CryBits.Client.Core;
using CryBits.Client.Framework.Assets;
using CryBits.Client.Framework.Audio;
using CryBits.Definitions.Maps;
using CryBits.Simulation.Components;
using CryBits.Simulation.Spatial;
using MapDef = CryBits.Definitions.Maps.Map;

namespace CryBits.Client.Systems.Map;

internal sealed class LightningSystem(GameContext context, AudioManager audioManager) : IClientSystem
{
    private static readonly string[] _thunderSounds = [Sounds.Thunder1, Sounds.Thunder2, Sounds.Thunder3, Sounds.Thunder4];

    private const float LightningDecayInterval = 0.025f;

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
        var weather = GetEffectiveWeather(map);
        if (weather == WeatherType.None) return;

        foreach (var state in context.World.All)
        {
            var lightning = state.Get<LightningComponent>();
            if (lightning == null) continue;

            if (lightning.Intensity > 0)
            {
                var newAccumulator = lightning.DecayAccumulator + dt;
                var newIntensity = lightning.Intensity;
                while (newAccumulator >= LightningDecayInterval)
                {
                    newAccumulator -= LightningDecayInterval;
                    newIntensity = newIntensity > 10 ? (byte)(newIntensity - 10) : (byte)0;
                }
                context.World.Set(state.Id, new LightningComponent(newIntensity, newAccumulator));
            }
        }

        if (weather == WeatherType.Thunder)
            TryThunder();
    }

    private void TryThunder()
    {
        if (Random.Shared.Next(0, 1000) != 0) return;

        var thunder = Random.Shared.Next(0, _thunderSounds.Length);
        audioManager.PlaySound(_thunderSounds[thunder]);

        if (thunder < 3)
        {
            foreach (var state in context.World.All)
            {
                var lightning = state.Get<LightningComponent>();
                if (lightning == null) continue;

                context.World.Set(state.Id, new LightningComponent(190, 0f));
            }
        }
    }
}
