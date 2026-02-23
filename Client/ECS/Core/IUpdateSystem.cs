namespace CryBits.Client.ECS;

/// <summary>
/// System that runs game logic every tick (or on a fixed interval).
/// Systems must be stateless — all mutable state lives in components.
/// </summary>
internal interface IUpdateSystem
{
    void Update(GameContext ctx);
}
