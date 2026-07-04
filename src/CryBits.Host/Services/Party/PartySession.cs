using CryBits.Simulation.State;

namespace CryBits.Host.Services.Party;

public sealed class PartySession(EntityId leader)
{
    public List<EntityId> Members { get; } = [leader];
    public EntityId Leader { get; set; } = leader;
}