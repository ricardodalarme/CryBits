namespace CryBits.Client.Components;

/// <summary>
/// Character attributes (Strength, Resistance, Intelligence, Agility, Vitality).
/// </summary>
internal struct AttributesComponent()
{
    /// <summary>Attribute values indexed by Enums.Attribute.</summary>
    public short[] Values = new short[(byte)Enums.Attribute.Count];
}
