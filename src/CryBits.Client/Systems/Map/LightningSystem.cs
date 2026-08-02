using CryBits.Client.Components;
using CryBits.Client.Core;
using CryBits.Client.Framework.Assets;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Replication;
using CryBits.Definitions.Maps;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Spatial;
using MapDef = CryBits.Definitions.Maps.Map;

namespace CryBits.Client.Systems.Map;

internal sealed class LightningSystem(ReplicationState replication, AudioManager audioManager) : IClientSystem
{
    private static readonly string[] _thunderSounds =
        [Sounds.Thunder1, Sounds.Thunder2, Sounds.Thunder3, Sounds.Thunder4];

    private const float LightningDecayInterval = 0.025f;

    private WeatherType GetEffectiveWeather(World world, MapDef map)
    {
        var playerId = replication.LocalPlayerEntity;
        if (playerId == null) return map.DefaultWeather;
        var pos = world.Get<Position>(playerId.Value);
        if (pos == null) return map.DefaultWeather;
        var chunkCoord = ChunkGrid.FromPosition(pos.X, pos.Y);
        if (map.Chunks.TryGetValue(chunkCoord, out var chunk) && chunk.WeatherOverride.HasValue)
            return chunk.WeatherOverride.Value;
        return map.DefaultWeather;
    }

    public void Update(World world, float dt)
    {
        var map = world.CurrentMap;
        if (map == null) return;
        var weather = GetEffectiveWeather(world, map);
        if (weather == WeatherType.None) return;

        foreach (var entityId in world.All)
        {
            var lightning = world.Get<LightningComponent>(entityId);
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

                world.Set(entityId, new LightningComponent(newIntensity, newAccumulator));
            }
        }

        if (weather == WeatherType.Thunder)
            TryThunder(world);
    }

    private void TryThunder(World world)
    {
        if (Random.Shared.Next(0, 1000) != 0) return;

        var thunder = Random.Shared.Next(0, _thunderSounds.Length);
        audioManager.PlaySound(_thunderSounds[thunder]);

        if (thunder < 3)
            foreach (var entityId in world.All)
            {
                var lightning = world.Get<LightningComponent>(entityId);
                if (lightning == null) continue;

                world.Set(entityId, new LightningComponent(190));
            }
    }
}
