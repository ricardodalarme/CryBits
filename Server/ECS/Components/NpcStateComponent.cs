namespace CryBits.Server.ECS.Components;

/// <summary>Runtime alive/spawn state of an NPC entity.</summary>
internal sealed class NpcStateComponent : ECS.IComponent
{
    public bool Alive;
    public long SpawnTimer;
}
