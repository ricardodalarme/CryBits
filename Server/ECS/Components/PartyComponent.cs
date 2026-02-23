using System.Collections.Generic;

namespace CryBits.Server.ECS.Components;

/// <summary>Party membership for a player.</summary>
internal sealed class PartyComponent : ECS.IComponent
{
    /// <summary>Entity IDs of all other party members (excludes self).</summary>
    public List<int> MemberEntityIds = [];

    /// <summary>Name of the player who sent a pending party invitation to this player.</summary>
    public string? PendingRequest;
}
