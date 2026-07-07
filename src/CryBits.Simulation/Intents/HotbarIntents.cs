using CryBits.Definitions.Items;
using CryBits.Simulation.Core;
using MemoryPack;

namespace CryBits.Simulation.Intents;

[MemoryPackable]
public sealed partial record HotbarAddIntent(EntityId SourceEntityId, short HotbarSlot, SlotType Type, short Slot) : Intent(SourceEntityId);
[MemoryPackable]
public sealed partial record HotbarSwapIntent(EntityId SourceEntityId, short SlotOld, short SlotNew) : Intent(SourceEntityId);
[MemoryPackable]
public sealed partial record HotbarUseIntent(EntityId SourceEntityId, short Slot) : Intent(SourceEntityId);
