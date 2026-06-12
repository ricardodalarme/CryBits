using CryBits.Server.Entities;
using CryBits.Simulation.Events;

namespace CryBits.Server.Simulation.Events;

internal sealed record PlayerWarpedEvent : SimEvent
{
    public Player Player { get; init; }
    public MapInstance OldMap { get; init; }
    public MapInstance NewMap { get; init; }
}
