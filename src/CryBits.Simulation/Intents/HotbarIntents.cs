using CryBits.Definitions.Items;
using CryBits.Simulation.State;

namespace CryBits.Simulation.Intents;

public sealed record HotbarAddIntent(EntityId SourceEntityId, short HotbarSlot, SlotType Type, short Slot) : Intent(SourceEntityId);
public sealed record HotbarSwapIntent(EntityId SourceEntityId, short SlotOld, short SlotNew) : Intent(SourceEntityId);
public sealed record HotbarUseIntent(EntityId SourceEntityId, short Slot) : Intent(SourceEntityId);
