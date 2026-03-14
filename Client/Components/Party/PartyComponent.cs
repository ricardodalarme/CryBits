using System;

namespace CryBits.Client.Components.Party;

/// <summary>Party membership for the local player.</summary>
internal struct PartyComponent()
{
    /// <summary>IDs of the active party members (excluding the local player).</summary>
    public Guid[] MemberIds = [];
}
