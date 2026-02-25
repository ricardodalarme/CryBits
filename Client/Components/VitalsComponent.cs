using CryBits.Enums;

namespace CryBits.Client.Components;

/// <summary>
/// Current and maximum vital values.
/// </summary>
internal struct VitalsComponent()
{
    public short[] Current { get; set; } = new short[(byte)Vital.Count];
    public short[] Max { get; set; } = new short[(byte)Vital.Count];
}
