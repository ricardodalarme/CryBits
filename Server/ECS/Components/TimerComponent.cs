namespace CryBits.Server.ECS.Components;

/// <summary>Attack cooldown timers for players and NPCs.</summary>
internal sealed class TimerComponent : ECS.IComponent
{
    public long AttackTimer;
}
