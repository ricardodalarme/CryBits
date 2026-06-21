using CryBits.Definitions.Maps;

namespace CryBits.Client.Components;

public sealed class WeatherParticleComponent
{
    public int Speed { get; set; }
    public int Start { get; set; }
    public bool Back { get; set; }
    public Weather Type { get; set; }
}
