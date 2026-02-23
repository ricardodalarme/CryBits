using CryBits.Entities;

namespace CryBits.Server.ECS.Components;

/// <summary>Core player identity and progression data.</summary>
internal sealed class PlayerDataComponent : ECS.IComponent
{
    public string Name = string.Empty;
    public Class? Class;
    public short TextureNum;
    public bool Genre;
    public short Level;
    public int Experience;
    public byte Points;
}
