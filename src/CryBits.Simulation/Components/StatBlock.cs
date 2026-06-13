namespace CryBits.Simulation.Components;

public sealed class StatBlock
{
    public short Level { get; set; }
    public int Experience { get; set; }
    public byte Points { get; set; }
    public short[] Attribute { get; set; } = new short[(byte)CryBits.Definitions.Characters.Attribute.Count];
}
