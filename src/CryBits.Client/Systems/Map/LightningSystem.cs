using Arch.Core;
using Arch.System;
using CryBits.Client.Components.Map;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Worlds;
using CryBits.Enums;
using static CryBits.Globals;
using static CryBits.Utils.RandomUtils;

namespace CryBits.Client.Systems.Map;

/// <summary>
/// Manages the lightning flash lifecycle and thunder audio for thunderstorm maps.
/// </summary>
internal sealed class LightningSystem(GameContext context, AudioManager audioManager) : BaseSystem<World, float>(context.World)
{
    private readonly QueryDescription _lightningQuery =
        new QueryDescription().WithAll<LightningComponent>();

    private static readonly string[] _thunderSounds = [Sounds.Thunder1, Sounds.Thunder2, Sounds.Thunder3, Sounds.Thunder4];

    /// <summary>Seconds between each 10-unit lightning intensity decay step (25 ms).</summary>
    private const float LightningDecayInterval = 0.025f;

    public override void Update(in float dt)
    {
        var weatherData = context.CurrentMap?.Data.Weather;
        if (weatherData == null || weatherData.Type == Weather.Normal) return;

        var delta = dt;
        World.Query(in _lightningQuery, (ref LightningComponent lightning) =>
        {
            if (lightning.Intensity > 0)
            {
                lightning.DecayAccumulator += delta;
                while (lightning.DecayAccumulator >= LightningDecayInterval)
                {
                    lightning.DecayAccumulator -= LightningDecayInterval;
                    lightning.Intensity = lightning.Intensity > 10 ? (byte)(lightning.Intensity - 10) : (byte)0;
                }
            }
        });

        if (weatherData.Type == Weather.Thundering)
            TryThunder(weatherData.Intensity);
    }

    private void TryThunder(byte intensity)
    {
        if (MyRandom.Next(0, MaxWeatherIntensity * 10 - intensity * 2) != 0) return;

        var thunder = MyRandom.Next(0, _thunderSounds.Length);
        audioManager.PlaySound(_thunderSounds[thunder]);

        if (thunder < 3)
        {
            World.Query(in _lightningQuery, (ref LightningComponent lightning) =>
            {
                lightning.Intensity = 190;
                lightning.DecayAccumulator = 0f;
            });
        }
    }
}
