using CryBits.Definitions.Slots;
using System;

namespace CryBits.Definitions.Npcs;

[Serializable]
public class NpcDrop(Guid itemId, short amount, byte chance) : ItemSlot(itemId, amount)
{
    public byte Chance { get; set; } = chance;
}
