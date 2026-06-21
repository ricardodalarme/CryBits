using CryBits.Client.Components;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Constants;
using CryBits.Definitions.Maps;
using CryBits.Simulation.Core;

namespace CryBits.Client.Spawners;

internal static class WeatherSpawner
{
    public static void Reset(World world, Weather weatherType)
    {
        world.DestroyWhere(s => s.Has<WeatherParticleComponent>());
        world.DestroyWhere(s => s.Has<LightningComponent>());

        AudioManager.Instance.StopAllSounds();

        switch (weatherType)
        {
            case Weather.Thundering:
                AudioManager.Instance.PlaySound(Sounds.Rain, true);
                _ = world.SpawnBuilder()
                    .With(new LightningComponent())
                    .Id;
                break;

            case Weather.Raining:
                AudioManager.Instance.PlaySound(Sounds.Rain, true);
                break;
        }
    }
}
