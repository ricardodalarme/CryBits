using System;
using CryBits.Enums;

namespace CryBits.Entities.Map;

/// <summary>
/// Represents a map layer and handles autotile calculations.
/// </summary>
[Serializable]
public class MapLayer
{
    public string Name { get; set; }
    public byte Type { get; set; }
    public MapTileData[,] Tile { get; set; } = new MapTileData[Map.Width, Map.Height];

    public MapLayer(string name)
    {
        Name = name;

        for (byte x = 0; x < Map.Width; x++)
            for (byte y = 0; y < Map.Height; y++)
                Tile[x, y] = new MapTileData();
    }

    /// <summary>
    /// Update nearby tiles and recalculate autotile parts.
    /// </summary>
    public void Update(int x, int y)
    {
        for (var x2 = x - 2; x2 < x + 2; x2++)
            for (var y2 = y - 2; y2 < y + 2; y2++)
                if (x2 >= 0 && x2 < Map.Width && y2 >= 0 && y2 < Map.Height)
                    Calculate((byte)x2, (byte)y2);
    }

    private bool Check(int x1, int y1, int x2, int y2)
    {
        // Out of bounds is treated as "not available" for autotiling checks
        if (x1 < 0 || x1 >= Map.Width || y1 < 0 || y1 >= Map.Height) return true;
        if (x2 < 0 || x2 >= Map.Width || y2 < 0 || y2 >= Map.Height) return true;

        var data1 = Tile[x1, y1];
        var data2 = Tile[x2, y2];

        // Return true when the neighbour is a matching autotile source
        if (!data2.IsAutoTile) return false;
        if (data1.Texture != data2.Texture) return false;
        if (data1.X != data2.X) return false;
        if (data1.Y != data2.Y) return false;

        return true;
    }

    public void Calculate(byte x, byte y)
    {
        CalculateNw(x, y);
        CalculateNe(x, y);
        CalculateSw(x, y);
        CalculateSe(x, y);
    }

    private void CalculateNw(byte x, byte y)
    {
        var available = new bool[3];
        var mode = AddMode.None;

        // Check neighboring tiles (northwest, north, west).
        if (Check(x, y, x - 1, y - 1)) available[0] = true;
        if (Check(x, y, x, y - 1)) available[1] = true;
        if (Check(x, y, x - 1, y)) available[2] = true;

        // Determine which quadrant mode to use.
        if (!available[1] && !available[2]) mode = AddMode.Inside;
        if (!available[1] && available[2]) mode = AddMode.Horizontal;
        if (available[1] && !available[2]) mode = AddMode.Vertical;
        if (!available[0] && available[1] && available[2]) mode = AddMode.Exterior;
        if (available[0] && available[1] && available[2]) mode = AddMode.Fill;

        // Set mini-tile
        switch (mode)
        {
            case AddMode.Inside: Tile[x, y].SetMini(0, "e"); break;
            case AddMode.Exterior: Tile[x, y].SetMini(0, "a"); break;
            case AddMode.Horizontal: Tile[x, y].SetMini(0, "i"); break;
            case AddMode.Vertical: Tile[x, y].SetMini(0, "m"); break;
            case AddMode.Fill: Tile[x, y].SetMini(0, "q"); break;
        }
    }

    private void CalculateNe(byte x, byte y)
    {
        var available = new bool[3];
        var mode = AddMode.None;

        // Check neighboring tiles (north, northeast, east).
        if (Check(x, y, x, y - 1)) available[0] = true;
        if (Check(x, y, x + 1, y - 1)) available[1] = true;
        if (Check(x, y, x + 1, y)) available[2] = true;

        // Determine which quadrant mode to use.
        if (!available[0] && !available[2]) mode = AddMode.Inside;
        if (!available[0] && available[2]) mode = AddMode.Horizontal;
        if (available[0] && !available[2]) mode = AddMode.Vertical;
        if (available[0] && !available[1] && available[2]) mode = AddMode.Exterior;
        if (available[0] && available[1] && available[2]) mode = AddMode.Fill;

        // Set mini-tile
        switch (mode)
        {
            case AddMode.Inside: Tile[x, y].SetMini(1, "j"); break;
            case AddMode.Exterior: Tile[x, y].SetMini(1, "b"); break;
            case AddMode.Horizontal: Tile[x, y].SetMini(1, "f"); break;
            case AddMode.Vertical: Tile[x, y].SetMini(1, "r"); break;
            case AddMode.Fill: Tile[x, y].SetMini(1, "n"); break;
        }
    }

    private void CalculateSw(byte x, byte y)
    {
        var available = new bool[3];
        var mode = AddMode.None;

        // Check neighboring tiles (west, southwest, south).
        if (Check(x, y, x - 1, y)) available[0] = true;
        if (Check(x, y, x - 1, y + 1)) available[1] = true;
        if (Check(x, y, x, y + 1)) available[2] = true;

        // Determine which quadrant mode to use.
        if (!available[0] && !available[2]) mode = AddMode.Inside;
        if (available[0] && !available[2]) mode = AddMode.Horizontal;
        if (!available[0] && available[2]) mode = AddMode.Vertical;
        if (available[0] && !available[1] && available[2]) mode = AddMode.Exterior;
        if (available[0] && available[1] && available[2]) mode = AddMode.Fill;

        // Set mini-tile
        switch (mode)
        {
            case AddMode.Inside: Tile[x, y].SetMini(2, "o"); break;
            case AddMode.Exterior: Tile[x, y].SetMini(2, "c"); break;
            case AddMode.Horizontal: Tile[x, y].SetMini(2, "s"); break;
            case AddMode.Vertical: Tile[x, y].SetMini(2, "g"); break;
            case AddMode.Fill: Tile[x, y].SetMini(2, "k"); break;
        }
    }

    private void CalculateSe(byte x, byte y)
    {
        var available = new bool[3];
        var mode = AddMode.None;

        // Check neighboring tiles (south, southeast, east).
        if (Check(x, y, x, y + 1)) available[0] = true;
        if (Check(x, y, x + 1, y + 1)) available[1] = true;
        if (Check(x, y, x + 1, y)) available[2] = true;

        // Determine which quadrant mode to use.
        if (!available[0] && !available[2]) mode = AddMode.Inside;
        if (!available[0] && available[2]) mode = AddMode.Horizontal;
        if (available[0] && !available[2]) mode = AddMode.Vertical;
        if (available[0] && !available[1] && available[2]) mode = AddMode.Exterior;
        if (available[0] && available[1] && available[2]) mode = AddMode.Fill;

        // Set mini-tile
        switch (mode)
        {
            case AddMode.Inside: Tile[x, y].SetMini(3, "t"); break;
            case AddMode.Exterior: Tile[x, y].SetMini(3, "d"); break;
            case AddMode.Horizontal: Tile[x, y].SetMini(3, "p"); break;
            case AddMode.Vertical: Tile[x, y].SetMini(3, "l"); break;
            case AddMode.Fill: Tile[x, y].SetMini(3, "h"); break;
        }
    }
}
