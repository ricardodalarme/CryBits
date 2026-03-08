using Arch.Core;
using CryBits.Client.Components.Map;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Constants;
using CryBits.Enums;

namespace CryBits.Client.Spawners;

/// <summary>
/// Resets all weather-related ECS state for the current map.
///
/// Call on map load or whenever the weather type changes (mirrors the role of
/// <c>MapWeatherInstance.UpdateType()</c>).
///
/// What this does:
///   1. Destroys every existing <see cref="WeatherParticleComponent"/> entity.
///   2. Destroys the <see cref="LightningComponent"/> singleton entity (if any).
///   3. Stops all ambient weather audio.
///   4. Starts the rain loop and creates the lightning entity for storm maps.
///
/// New particles are spawned incrementally each frame by <c>WeatherSimulationSystem</c>.
/// </summary>
internal static class WeatherSpawner
{
    private static readonly QueryDescription _particleQuery =
        new QueryDescription().WithAll<WeatherParticleComponent>();

    private static readonly QueryDescription _lightningQuery =
        new QueryDescription().WithAll<LightningComponent>();

    private static readonly AudioManager _audioManager = AudioManager.Instance;

    public static void Reset(World world, Weather weatherType)
    {
        // Tear down all existing weather entities.
        world.Destroy(in _particleQuery);
        world.Destroy(in _lightningQuery);

        // Always stop ambient weather sounds before potentially starting new ones.
        _audioManager.StopAllSounds();

        switch (weatherType)
        {
            case Weather.Thundering:
                _audioManager.PlaySound(Sounds.Rain, true);
                world.Create(new LightningComponent());
                break;

            case Weather.Raining:
                _audioManager.PlaySound(Sounds.Rain, true);
                break;
        }
    }
}
