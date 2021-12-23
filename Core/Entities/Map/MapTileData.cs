using System;
using System.Drawing;

namespace CryBits.Entities.Map;

[Serializable]
public class MapTileData
{
    public byte X { get; set; }
    public byte Y { get; set; }
    public byte Texture { get; set; }
    public bool IsAutoTile { get; set; }
    public Point[] Mini { get; set; } = new Point[4];

    public void SetMini(byte index, string mode)
    {
        var position = new Point(0);

        // Posições exatas dos mini azulejos (16x16)
        switch (mode)
        {
            // Quinas
            case "a": position = new Point(32, 0); break;
            case "b": position = new Point(48, 0); break;
            case "c": position = new Point(32, 16); break;
            case "d": position = new Point(48, 16); break;

            // Noroeste
            case "e": position = new Point(0, 32); break;
            case "f": position = new Point(16, 32); break;
            case "g": position = new Point(0, 48); break;
            case "h": position = new Point(16, 48); break;

            // Nordeste
            case "i": position = new Point(32, 32); break;
            case "j": position = new Point(48, 32); break;
            case "k": position = new Point(32, 48); break;
            case "l": position = new Point(48, 48); break;

            // Sudoeste
            case "m": position = new Point(0, 64); break;
            case "n": position = new Point(16, 64); break;
            case "o": position = new Point(0, 80); break;
            case "p": position = new Point(16, 80); break;

            // Sudeste
            case "q": position = new Point(32, 64); break;
            case "r": position = new Point(48, 64); break;
            case "s": position = new Point(32, 80); break;
            case "t": position = new Point(48, 80); break;
        }

        // Define a posição do mini azulejo
        Mini[index].X = (X * Globals.Grid) + position.X;
        Mini[index].Y = (Y * Globals.Grid) + position.Y;
    }
}