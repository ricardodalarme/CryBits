using CryBits.Client.Components;
using CryBits.Client.Framework.Assets;
using CryBits.Client.Framework.Audio;
using CryBits.Definitions.Maps;
using CryBits.Simulation.Core;

namespace CryBits.Client.Spawners;

internal static class WeatherSpawner
{
    public static void Reset(World world, WeatherType weatherType, AudioManager audioManager)
    {
        world.DestroyWhere(world.Has<WeatherParticleComponent>);
        world.DestroyWhere(world.Has<LightningComponent>);

        audioManager.StopAllSounds();

        switch (weatherType)
        {
            case WeatherType.Thunder:
                audioManager.PlaySound(Sounds.Rain, true);
                _ = world.SpawnBuilder()
                    .With(new LightningComponent())
                    .Id;
                break;

            case WeatherType.Rain:
                audioManager.PlaySound(Sounds.Rain, true);
                break;
        }
    }
}
