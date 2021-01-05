using CryBits.Editors.Library;
using SFML.Graphics;

namespace CryBits.Editors.Media.Graphics
{
    internal static class Fonts
    {
        public static Font Default;

        public static void LoadAll()
        {
            Default = new Font(Directories.Fonts.FullName + "Georgia.ttf");
        }
    }
}