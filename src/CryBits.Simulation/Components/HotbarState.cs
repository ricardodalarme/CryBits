using CryBits.Definitions.Items;
using CryBits.Definitions.Slots;
using System.Linq;
using static CryBits.Definitions.Globals;

namespace CryBits.Simulation.Components;

public sealed class HotbarState
{
    public HotbarSlot[] Slots { get; set; } = new HotbarSlot[MaxHotbar];

    public HotbarSlot? Find(SlotType type, short slot) =>
        Slots.FirstOrDefault(x => x.Type == type && x.Slot == slot);
}
