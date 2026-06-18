using MemoryPack;

namespace CryBits.Definitions.Npcs;

[MemoryPackable]
public partial class NpcDrop(Guid itemId, short amount, byte chance)
{
    public Guid ItemId { get; set; } = itemId;
    public short Amount { get; set; } = amount;
    public byte Chance { get; set; } = chance;
}
