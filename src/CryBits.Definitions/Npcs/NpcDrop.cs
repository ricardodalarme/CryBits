using System;

namespace CryBits.Definitions.Npcs;

[Serializable]
public class NpcDrop(Guid itemId, short amount, byte chance)
{
    public Guid ItemId { get; set; } = itemId;
    public short Amount { get; set; } = amount;
    public byte Chance { get; set; } = chance;
}
