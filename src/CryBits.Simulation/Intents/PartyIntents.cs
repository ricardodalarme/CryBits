using CryBits.Simulation.State;

namespace CryBits.Simulation.Intents;

public sealed record PartyInviteIntent(EntityId SourceEntityId, string PlayerName) : Intent(SourceEntityId);
public sealed record PartyAcceptIntent(EntityId SourceEntityId) : Intent(SourceEntityId);
public sealed record PartyDeclineIntent(EntityId SourceEntityId) : Intent(SourceEntityId);
public sealed record PartyLeaveIntent(EntityId SourceEntityId) : Intent(SourceEntityId);
