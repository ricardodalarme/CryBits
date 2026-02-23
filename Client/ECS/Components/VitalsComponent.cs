using CryBits.Enums;

namespace CryBits.Client.ECS.Components;

/// <summary>
/// Current and maximum vital values (HP, MP, …) synced from the server.
/// Index with <c>(byte)Vital.Hp</c> etc.
/// </summary>
public sealed class VitalsComponent : IComponent
{
    public short[] Current { get; set; } = new short[(byte)Vital.Count];
    public short[] Max { get; set; } = new short[(byte)Vital.Count];
}
