using CryBits.Client.Components;
using CryBits.Client.Core;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Worlds;
using CryBits.Definitions.Maps;
using static CryBits.Definitions.Globals;

namespace CryBits.Client.Systems.Map;

internal sealed class LightningSystem(GameContext context, AudioManager audioManager) : IClientSystem
{
    private static readonly string[] _thunderSounds = [Sounds.Thunder1, Sounds.Thunder2, Sounds.Thunder3, Sounds.Thunder4];

    private const float LightningDecayInterval = 0.025f;

    public void Update(float dt)
    {
        var weatherData = context.CurrentMap?.Data.Weather;
        if (weatherData == null || weatherData.Type == Weather.Normal) return;

        foreach (var state in context.World.All)
        {
            var lightning = state.Get<LightningComponent>();
            if (lightning == null) continue;

            if (lightning.Intensity > 0)
            {
                lightning.DecayAccumulator += dt;
                while (lightning.DecayAccumulator >= LightningDecayInterval)
                {
                    lightning.DecayAccumulator -= LightningDecayInterval;
                    lightning.Intensity = lightning.Intensity > 10 ? (byte)(lightning.Intensity - 10) : (byte)0;
                }
            }
        }

        if (weatherData.Type == Weather.Thundering)
            TryThunder(weatherData.Intensity);
    }

    private void TryThunder(byte intensity)
    {
        if (Random.Shared.Next(0, MaxWeatherIntensity * 10 - intensity * 2) != 0) return;

        var thunder = Random.Shared.Next(0, _thunderSounds.Length);
        audioManager.PlaySound(_thunderSounds[thunder]);

        if (thunder < 3)
        {
            foreach (var state in context.World.All)
            {
                var lightning = state.Get<LightningComponent>();
                if (lightning == null) continue;

                lightning.Intensity = 190;
                lightning.DecayAccumulator = 0f;
            }
        }
    }
}
