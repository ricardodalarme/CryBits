using CryBits.Definitions.Items;
using CryBits.Definitions.Slots;
using System;

namespace CryBits.Definitions.Npcs;

[Serializable]
public class NpcDrop(Item item, short amount, byte chance) : ItemSlot(item, amount)
{
    public byte Chance { get; set; } = chance;
}
