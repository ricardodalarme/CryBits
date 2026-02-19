using CryBits.Client.Framework.Graphics;
using SFML.Graphics;

namespace CryBits.Client.Utils;

internal static class TextUtils
{
    public static short MeasureString(string text)
    {
        // Measure string width using SFML Text.
        var tempText = new Text(Fonts.Default, text) { CharacterSize = 10 };
        return (short)tempText.GetLocalBounds().Width;
    }

    public static string TextBreak(string text, int width)
    {
        // Return early for empty input.
        if (string.IsNullOrEmpty(text)) return text;

        // Measure current text width.
        int textWidth = MeasureString(text);

        // Trim leading characters until the text fits.
        while (textWidth - width >= 0)
        {
            text = text.Substring(1);
            textWidth = MeasureString(text);
        }

        return text;
    }
}