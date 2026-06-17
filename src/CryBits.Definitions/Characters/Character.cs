namespace CryBits.Definitions.Characters;

public sealed class Character
{
    public string Name { get; set; } = string.Empty;
    public Guid ClassId { get; set; }
    public Gender Gender { get; set; }
    public short TextureNum { get; set; }
    public short Level { get; set; } = 1;
    public int Experience { get; set; }
    public byte Points { get; set; }
    public short[] Attributes { get; set; } = [];
    public Guid MapId { get; set; }
    public byte X { get; set; }
    public byte Y { get; set; }
    public byte Direction { get; set; }
    public short Hp { get; set; }
    public short Mp { get; set; }

    public Guid[] InventoryIds { get; set; } = [];
    public short[] InventoryAmounts { get; set; } = [];
    public Guid[] Equipment { get; set; } = [];
    public byte[] HotbarTypes { get; set; } = [];
    public byte[] HotbarSlots { get; set; } = [];
}
