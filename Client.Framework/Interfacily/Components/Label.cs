using CryBits.Enums;

namespace CryBits.Client.Framework.Interfacily.Components;

public class Label : Component
{
    public string Text { get; set; } = string.Empty;
    public int Color { get; set; } = 0xFFFFFF;
    public TextAlign Alignment { get; set; } = TextAlign.Left;
    public int MaxWidth { get; set; } = 0;

    private object[] _args = Array.Empty<object>();

    /// <summary>Stores live values to be interpolated into <see cref="Text"/> at render time.</summary>
    public void SetArguments(params object[] args) => _args = args;

    /// <summary>Returns <see cref="Text"/> formatted with the stored args, or the raw text when no args are set.</summary>
    public string FormattedText() => _args.Length > 0 ? string.Format(Text, _args) : Text;

    public override string ToString() => "[Label] " + Name;
}
