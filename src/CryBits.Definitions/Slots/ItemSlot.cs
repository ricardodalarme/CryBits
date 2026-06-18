using MemoryPack;

namespace CryBits.Definitions.Slots;

[MemoryPackable]
public partial record struct ItemSlot(Guid ItemId, short Amount);
