using CryBits.Definitions.Items;

namespace CryBits.Definitions.Slots;

public class HotbarSlot(SlotType type, short slot)
{
    public SlotType Type { get; set; } = type;
    public short Slot { get; set; } = slot;
}
