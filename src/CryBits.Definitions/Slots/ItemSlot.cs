using System;

namespace CryBits.Definitions.Slots;

[Serializable]
public record struct ItemSlot(Guid ItemId, short Amount);
