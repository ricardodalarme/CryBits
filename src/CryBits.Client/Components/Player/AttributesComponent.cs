namespace CryBits.Client.Components.Player;

/// <summary>
/// Character attributes (Strength, Resistance, Intelligence, Agility, Vitality).
/// </summary>
internal struct AttributesComponent()
{
    /// <summary>Attribute values indexed by CryBits.Definitions.Characters.Attribute.</summary>
    public short[] Values = new short[(byte)CryBits.Definitions.Characters.Attribute.Count];
}
