using CryBits.Server.Entities;
using CryBits.Simulation.Events;

namespace CryBits.Server.Simulation.Events;

internal sealed record NpcAttackedEvent : SimEvent
{
    public Player Attacker { get; init; }
    public NpcInstance Npc { get; init; }
}
