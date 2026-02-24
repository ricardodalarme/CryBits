using SFML.Graphics;

namespace CryBits.Client.Components;

/// <summary>Draws static text with an offset from the entity's root position.</summary>
internal struct TextComponent(string text, Color color, int offsetX = 0, int offsetY = 0, bool centered = true)
{
    public string Text = text;
    public Color Color = color;
    public int OffsetX = offsetX;
    public int OffsetY = offsetY;
    public bool Centered = centered;
}
