using CryBits.Client.Library;
using SFML.Graphics;

namespace CryBits.Client.Media.Graphics;

internal static class Fonts
{
    public static Font Default;

    public static void LoadAll()
    {
        Default = new Font(Directories.Fonts.FullName + "Georgia.ttf");
    }
}