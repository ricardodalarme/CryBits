using CryBits.Enums;

namespace CryBits.Client.Framework.Entities.Tile;

[Serializable]
public class TileData
{
    public byte Attribute { get; set; }
    public bool[] Block { get; set; } = new bool[(byte) Direction.Count];
}