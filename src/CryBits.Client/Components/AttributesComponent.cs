namespace CryBits.Client.Components;

public sealed class AttributesComponent
{
    public short[] Values { get; set; } = new short[(byte)CryBits.Definitions.Characters.Attribute.Count];
}
