using System;
using System.Collections.Generic;

namespace Entities
{
    [Serializable]
    class Map : Entity
    {
        // Lista de dados
        public static Dictionary<Guid, Map> List;

        // Obtém o dado, caso ele não existir retorna nulo
        public static Map Get(Guid ID) => List.ContainsKey(ID) ? List[ID] : null;

        // Dados gerais
        public short Revision;
        public string Name;
        public byte Moral;
        public byte Panorama;
        public byte Music;
        public int Color;
        public Map_Weather Weather;
        public Map_Fog Fog;
        public short[] Link;
        public Map_Tile[,] Tile = new Map_Tile[Map_Width, Map_Height];
        public Map_Light[] Light;
        public short[] NPC;

        public Map(Guid ID) : base(ID) { }
    }

    [Serializable]
    public struct Map_Weather
    {
        public byte Type;
        public byte Intensity;
    }

    [Serializable]
    public struct Map_Fog
    {
        public byte Texture;
        public sbyte Speed_X;
        public sbyte Speed_Y;
        public byte Alpha;
    }

    [Serializable]
    public struct Map_Tile
    {
        public byte Attribute;
        public bool[] Block;
        public Map_Tile_Data[,] Data;
    }

    [Serializable]
    public struct Map_Tile_Data
    {
        public byte X;
        public byte Y;
        public byte Tile;
        public bool Automatic;
        public System.Drawing.Point[] Mini;
    }

    [Serializable]
    public struct Map_Light
    {
        public byte X;
        public byte Y;
        public byte Width;
        public byte Height;
    }
}