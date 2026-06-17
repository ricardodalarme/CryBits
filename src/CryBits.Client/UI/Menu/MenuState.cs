namespace CryBits.Client.UI.Menu;

internal static class MenuState
{
    public struct TempCharacter
    {
        public string Name;
        public short TextureNum;
    }

    public static TempCharacter[]? Characters;
    public static int CurrentCharacter;
    public static byte CurrentClass;
    public static byte CurrentTexture;
}
