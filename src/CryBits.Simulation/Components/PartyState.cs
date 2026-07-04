using CryBits.Simulation.State;
using MemoryPack;

namespace CryBits.Simulation.Components;

[MemoryPackable]
public sealed partial record PartyState(List<EntityId> Members, EntityId? PendingInviterId);
