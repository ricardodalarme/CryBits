using CryBits.Enums;

namespace CryBits.Client.Components.Combat;

/// <summary>
/// Current and maximum vital values.
/// </summary>
internal struct VitalsComponent()
{
    public short[] Current = new short[(byte)Vital.Count];
    public short[] Max = new short[(byte)Vital.Count];
}
