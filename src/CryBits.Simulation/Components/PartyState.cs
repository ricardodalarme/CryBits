using CryBits.Simulation.State;
using MemoryPack;

namespace CryBits.Simulation.Components;

[MemoryPackable]
public sealed partial record class PartyState(List<EntityId> Members, EntityId? PendingInviterId);
