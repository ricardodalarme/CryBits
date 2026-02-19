using CryBits.Client.Framework.Constants;
using SFML.Graphics;

namespace CryBits.Client.Framework.Graphics;

public static class Fonts
{
    public static readonly Font Default = new(Path.Combine(Directories.Fonts.FullName, "Georgia.ttf"));
}
