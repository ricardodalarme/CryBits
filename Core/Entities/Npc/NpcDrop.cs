using CryBits.Entities.Slots;
using System;

namespace CryBits.Entities.Npc;

[Serializable]
public class NpcDrop(Item item, short amount, byte chance) : ItemSlot(item, amount)
{
    public byte Chance { get; set; } = chance;
}
