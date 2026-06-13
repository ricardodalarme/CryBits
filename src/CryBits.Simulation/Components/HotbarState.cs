using CryBits.Definitions.Slots;
using static CryBits.Definitions.Globals;

namespace CryBits.Simulation.Components;

public sealed class HotbarState
{
    public HotbarSlot[] Slots { get; set; } = new HotbarSlot[MaxHotbar];
}
