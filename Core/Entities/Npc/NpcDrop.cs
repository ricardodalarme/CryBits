using System;
using CryBits.Entities.Slots;

namespace CryBits.Entities.Npc;

[Serializable]
public class NpcDrop : ItemSlot
{
    public byte Chance { get; set; }

    public NpcDrop(Item item, short amount, byte chance) : base(item, amount)
    {
        Chance = chance;
    }
}