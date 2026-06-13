using System.Collections.Generic;

namespace CryBits.Server.Simulation.State.Components;

internal sealed class PartyState
{
    public List<EntityId> Members { get; set; } = [];
    public string Request { get; set; } = string.Empty;
}
