using CryBits.Simulation.State;

namespace CryBits.Simulation.Intents;

public abstract record Intent(EntityId SourceEntityId)
{
    public long AcceptedTick { get; init; }
}
