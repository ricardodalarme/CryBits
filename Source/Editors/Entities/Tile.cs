using System;

namespace Entities
{
    [Serializable]
    public class Tile
    {
        public byte Width;
        public byte Height;
        public Tile_Data[,] Data;
    }

    [Serializable]
    public class Tile_Data
    {
        public byte Attribute;
        public bool[] Block = new bool[(byte)Utils.Directions.Count];
    }
}
