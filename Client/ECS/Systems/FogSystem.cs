namespace CryBits.Client.ECS.Systems;

/// <summary>
/// Delegates fog-scrolling simulation to <c>MapInstance.Fog.Update()</c>.
/// Kept as a system so the update ordering in <see cref="Loop"/> is explicit.
/// </summary>
internal sealed class FogSystem : IUpdateSystem
{
    public void Update(GameContext ctx)
    {
        ctx.CurrentMap?.Fog.Update();
    }
}
