using CryBits.Enums;

namespace CryBits.Client.Components.Map;

/// <summary>
/// Per-particle simulation state for weather effects (rain, snow, thunderstorm).
///
/// Position lives in <c>TransformComponent</c> so the render pipeline always reads
/// from a single source of truth. This component holds only the physics metadata
/// that the renderer does not need.
///
/// <list type="bullet">
///   <item><c>Speed</c> — pixels moved per simulation step.</item>
///   <item><c>Start</c> — snow-only: the horizontal origin used to compute the
///     left/right oscillation limits.</item>
///   <item><c>Back</c> — snow-only: current horizontal drift direction
///     (<c>true</c> = moving left).</item>
///   <item><c>Type</c> — determines movement formula and texture source rect.</item>
/// </list>
/// </summary>
internal struct WeatherParticleComponent
{
    /// <summary>Movement speed in pixels per simulation step.</summary>
    public int Speed;

    /// <summary>
    /// Snow only: horizontal starting position used to compute oscillation limits.
    /// Ignored for rain/thunderstorm particles.
    /// </summary>
    public int Start;

    /// <summary>
    /// Snow only: horizontal drift direction.
    /// <c>true</c> = drifting left; <c>false</c> = drifting right.
    /// </summary>
    public bool Back;

    /// <summary>Particle type — selects movement logic and sprite source rect.</summary>
    public Weather Type;
}
