using CryBits.Server.World;

namespace CryBits.Server.ECS.Components;

/// <summary>Links a player entity to its network session.</summary>
internal sealed class SessionComponent : ECS.IComponent
{
    public GameSession Session;

    public SessionComponent(GameSession session) => Session = session;
}
