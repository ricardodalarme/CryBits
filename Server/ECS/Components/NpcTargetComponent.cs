namespace CryBits.Server.ECS.Components;

/// <summary>
/// Current combat target for an NPC.
/// The entity ID may refer to either a player or another NPC.
/// Null when the NPC has no target.
/// </summary>
internal sealed class NpcTargetComponent : ECS.IComponent
{
    public int? TargetEntityId;
}
