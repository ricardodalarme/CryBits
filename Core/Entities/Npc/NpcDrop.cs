using System;
using CryBits.Entities.Slots;

namespace CryBits.Entities.Npc;

[Serializable]
public class NpcDrop(Item item, short amount, byte chance) : ItemSlot(item, amount)
{
    public byte Chance { get; set; } = chance;
}