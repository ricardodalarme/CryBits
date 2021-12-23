using CryBits.Enums;

namespace CryBits.Entities.Slots;

public class HotbarSlot
{
    public SlotType Type { get; set; }
    public short Slot { get; set; }

    public HotbarSlot(SlotType type, short slot)
    {
        Type = type;
        Slot = slot;
    }
}