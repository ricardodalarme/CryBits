using CryBits.Client.Framework.Graphics;
using SFML.Graphics;

namespace CryBits.Client.Framework.Utils;

public static class TextUtils
{
    // Reusing a single object avoids one heap allocation + GC pressure per call.
    private static readonly Text _measureCache = new(Fonts.Default, string.Empty) { CharacterSize = 10 };

    /// <summary>Returns the rendered pixel width of <paramref name="text"/>.</summary>
    public static short MeasureString(string text)
    {
        _measureCache.DisplayedString = text;
        return (short)_measureCache.GetLocalBounds().Width;
    }

    /// <summary>
    /// Finds the character index at which <paramref name="text"/> first exceeds
    /// <paramref name="maxWidth"/> pixels, or -1 if the entire string fits.
    public static int FindBreakIndex(string text, int maxWidth)
    {
        _measureCache.DisplayedString = text;
        var origin = _measureCache.FindCharacterPos(0).X;

        for (var i = 1; i <= text.Length; i++)
        {
            var advance = _measureCache.FindCharacterPos((uint)i).X - origin;
            if (advance > maxWidth) return i - 1;
        }

        return -1;
    }

    /// <summary>
    /// Trims characters from the start of <paramref name="text"/> until the
    /// remaining suffix fits within <paramref name="width"/> pixels.
    public static string TextBreak(string text, int width)
    {
        if (string.IsNullOrEmpty(text)) return text;

        _measureCache.DisplayedString = text;
        var origin = _measureCache.FindCharacterPos(0).X;
        var totalWidth = _measureCache.GetLocalBounds().Width;

        if (totalWidth < width) return text;

        for (var k = 1; k < text.Length; k++)
        {
            var prefixAdvance = _measureCache.FindCharacterPos((uint)k).X - origin;
            if (totalWidth - prefixAdvance < width) return text[k..];
        }

        return string.Empty;
    }
}
