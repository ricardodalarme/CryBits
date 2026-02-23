namespace CryBits.Client.ECS.Systems;

/// <summary>
/// Delegates weather-particle simulation to <c>MapInstance.Weather.Update()</c>.
/// Kept as a system so the update ordering in <see cref="Loop"/> is explicit.
/// </summary>
internal sealed class WeatherSystem : IUpdateSystem
{
    public void Update(GameContext ctx)
    {
        ctx.CurrentMap?.Weather.Update();
    }
}
