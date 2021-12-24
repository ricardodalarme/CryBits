using CryBits.Client.Framework.Interfacily.Components;

namespace CryBits.Client.Framework.Constants;

public static class Screens
{
    public static readonly Dictionary<string, Screen> List = new();
    public static Screen Menu => List["Menu"];
    public static Screen Game => List["Game"];
}