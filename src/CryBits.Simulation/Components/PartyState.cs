using CryBits.Simulation.State;
using System.Collections.Generic;

namespace CryBits.Simulation.Components;

public sealed class PartyState
{
    public List<EntityId> Members { get; set; } = [];
    public EntityId? PendingInviterId { get; set; }
}
