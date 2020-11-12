using CryBits.Entities;
using System;
using System.Collections.Generic;
using static CryBits.Client.Logic.Game;

namespace CryBits.Client.Entities
{
    [Serializable]
    class Map : Entity
    {
        // Lista de dados
        public static Dictionary<Guid, Map> List;

        // Obtém o dado, caso ele não existir retorna nulo
        public static Map Get(Guid id) => List.ContainsKey(id) ? List[id] : null;

        // Dados gerais
        public short Revision;
        public byte Moral;
        public byte Panorama;
        public byte Music;
        public int Color;
        public MapWeather Weather;
        public MapFog Fog;
        public short[] Link;
        public MapTile[,] Tile = new MapTile[MapWidth, MapHeight];
        public MapLight[] Light;
        public short[] NPC;

        public Map(Guid id) : base(id) { }
    }

    [Serializable]
    public struct MapWeather
    {
        public byte Type;
        public byte Intensity;
    }

    [Serializable]
    public struct MapFog
    {
        public byte Texture;
        public sbyte Speed_X;
        public sbyte Speed_Y;
        public byte Alpha;
    }

    [Serializable]
    public struct MapTile
    {
        public byte Attribute;
        public bool[] Block;
        public MapTileData[,] Data;
    }

    [Serializable]
    public struct MapTileData
    {
        public byte X;
        public byte Y;
        public byte Tile;
        public bool Automatic;
        public System.Drawing.Point[] Mini;
    }

    [Serializable]
    public struct MapLight
    {
        public byte X;
        public byte Y;
        public byte Width;
        public byte Height;
    }
}