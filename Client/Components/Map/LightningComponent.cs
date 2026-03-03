namespace CryBits.Client.Components.Map;

/// <summary>
/// Singleton component present only while the current map has thunderstorm weather.
/// A single entity carrying this component is created by <c>WeatherSpawner.Reset()</c>
/// and destroyed when the weather changes away from <c>Weather.Thundering</c>.
///
/// <c>Intensity</c> drives the alpha of the full-screen white flash overlay drawn by
/// <c>WeatherRenderSystem</c>. It starts at 190 on a strike and decays by 10 every 25 ms,
/// reaching zero in roughly 475 ms.
/// </summary>
internal struct LightningComponent
{
    /// <summary>
    /// Current flash alpha in the 0–255 range. 0 = no overlay.
    /// Decayed continuously at 400 units/second by <c>WeatherSimulationSystem</c>.
    /// </summary>
    public float Intensity;
}
