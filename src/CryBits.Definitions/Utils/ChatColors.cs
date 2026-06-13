namespace CryBits.Definitions.Utils;

public static class ChatColors
{
    private static int Rgb(int r, int g, int b) => (255 << 24) | (r << 16) | (g << 8) | b;

    public static int White => Rgb(255, 255, 255);
    public static int Red => Rgb(255, 0, 0);
    public static int Green => Rgb(0, 128, 0);
}
