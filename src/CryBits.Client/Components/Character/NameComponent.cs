using SFML.Graphics;

namespace CryBits.Client.Components.Character;

/// <summary>Identity name and display colour stored directly on the entity.</summary>
internal struct NameComponent
{
    public string Value;
    /// <summary>Colour used to render the name label above the character sprite.</summary>
    public Color NameColor;
}
