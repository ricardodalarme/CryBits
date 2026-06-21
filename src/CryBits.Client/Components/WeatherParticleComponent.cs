using CryBits.Definitions.Maps;

namespace CryBits.Client.Components;

public sealed record class WeatherParticleComponent(int Speed, int Start, bool Back, Weather Type);
