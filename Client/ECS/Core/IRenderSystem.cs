namespace CryBits.Client.ECS;

/// <summary>
/// System that issues draw calls every rendered frame.
/// Render systems read component data but must never mutate it.
/// </summary>
internal interface IRenderSystem
{
    void Render(GameContext ctx);
}
