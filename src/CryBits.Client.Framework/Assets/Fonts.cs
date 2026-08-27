using FontStashSharp;
using CryBits.Client.Framework.Constants;

namespace CryBits.Client.Framework.Assets;

/// <summary>
/// Static FontStashSharp font system used by MonoGame./>.
/// </summary>
public static class Fonts
{
    /// <summary>Default font used by the game.</summary>
    public static SpriteFontBase Default => System.GetFont(10);

    private static FontSystem? _system;

    /// <summary>Get the FontSystem instance, creating it on first access.</summary>
    public static FontSystem System
    {
        get
        {
            if (_system != null) return _system;
            _system = new FontSystem();
            _system.AddFont(File.ReadAllBytes(Path.Combine(Directories.Fonts.FullName, "Georgia.ttf")));
            return _system;
        }
    }
}
