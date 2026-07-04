using CryBits.Definitions.Maps;

namespace CryBits.Client.Components;

public sealed record WeatherParticleComponent(int Speed, int Start, bool Back, WeatherType Type);
