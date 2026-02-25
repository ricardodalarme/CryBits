namespace CryBits.Client.Components;

/// <summary>
/// Party membership for the local player.
/// </summary>
internal struct PartyComponent(int[] memberIds)
{
    /// <summary>Entity ids of the active party members (excluding the local player).</summary>
    public int[] MemberIds = memberIds;
}
