using CryBits.Client.Framework.Graphics;
using SFML.Graphics;

namespace CryBits.Client.Utils;

internal static class TextUtils
{
    public static short MeasureString(string text)
    {
        // Dados do texto
        var tempText = new Text(text, Fonts.Default) { CharacterSize = 10 };
        return (short)tempText.GetLocalBounds().Width;
    }

    public static string TextBreak(string text, int width)
    {
        // Previne sobrecargas
        if (string.IsNullOrEmpty(text)) return text;

        // Usado para fazer alguns calculosk
        int textWidth = MeasureString(text);

        // Diminui o tamanho do texto até que ele caiba no digitalizador
        while (textWidth - width >= 0)
        {
            text = text.Substring(1);
            textWidth = MeasureString(text);
        }

        return text;
    }
}