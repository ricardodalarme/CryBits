using CryBits.Simulation.Core;
using MemoryPack;

namespace CryBits.Simulation.Intents;

[MemoryPackable]
public sealed partial record PartyInviteIntent(EntityId SourceEntityId, string PlayerName) : Intent(SourceEntityId);
[MemoryPackable]
public sealed partial record PartyAcceptIntent(EntityId SourceEntityId) : Intent(SourceEntityId);
[MemoryPackable]
public sealed partial record PartyDeclineIntent(EntityId SourceEntityId) : Intent(SourceEntityId);
[MemoryPackable]
public sealed partial record PartyLeaveIntent(EntityId SourceEntityId) : Intent(SourceEntityId);
