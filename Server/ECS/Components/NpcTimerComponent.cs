namespace CryBits.Server.ECS.Components;

/// <summary>Attack cooldown timer for an NPC entity.</summary>
internal sealed class NpcTimerComponent : ECS.IComponent
{
    public long AttackTimer;
}
