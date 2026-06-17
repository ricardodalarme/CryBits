using CryBits.Definitions.Characters;

namespace CryBits.Simulation.Components;

public sealed class PlayerAppearance
{
    public string Name { get; set; } = string.Empty;
    public Guid ClassId { get; set; }
    public short TextureNum { get; set; }
    public Gender Gender { get; set; }
}
