using System;

namespace CryBits.Server.Simulation.State.Components;

internal sealed class PlayerAppearance
{
    public string Name { get; set; } = string.Empty;
    public Guid ClassId { get; set; }
    public short TextureNum { get; set; }
    public bool Genre { get; set; }
}
