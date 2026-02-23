using CryBits.Entities.Slots;
using CryBits.Enums;
using static CryBits.Globals;

namespace CryBits.Server.ECS.Components;

/// <summary>Player hotbar — pre-allocated slot array, mutated in place.</summary>
internal sealed class HotbarComponent : ECS.IComponent
{
    public HotbarSlot[] Slots;

    public HotbarComponent()
    {
        Slots = new HotbarSlot[MaxHotbar];
        for (var i = 0; i < Slots.Length; i++)
            Slots[i] = new HotbarSlot(SlotType.None, 0);
    }
}
