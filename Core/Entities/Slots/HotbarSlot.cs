using CryBits.Enums;

namespace CryBits.Entities.Slots;

public class HotbarSlot(SlotType type, short slot)
{
    public SlotType Type { get; set; } = type;
    public short Slot { get; set; } = slot;
}
