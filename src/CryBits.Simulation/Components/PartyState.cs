using CryBits.Simulation.State;

namespace CryBits.Simulation.Components;

public sealed class PartyState
{
    public List<EntityId> Members { get; set; } = [];
    public EntityId? PendingInviterId { get; set; }
}
