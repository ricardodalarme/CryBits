using System;
using System.Collections.Generic;
using System.Drawing;

namespace CryBits.Entities
{
    [Serializable]
    public class Map : Entity
    {
        // Lista de dados
        public static Dictionary<Guid, Map> List = new Dictionary<Guid, Map>();

        // Obtém o dado, caso ele não existir retorna nulo
        public static Map Get(Guid ID) => List.ContainsKey(ID) ? List[ID] : null;

        // Tamanho dos mapas
        public const byte Width = 25;
        public const byte Height = 19;

        // Dados
        public short Revision;
        public Map_Morals Moral;
        public IList<MapLayer> Layer = Array.Empty<MapLayer>();
        public MapAttribute[,] Attribute = new MapAttribute[Width, Height];
        public byte Panorama;
        public byte Music;
        public int Color = -1;
        public MapWeather Weather = new MapWeather();
        public MapFog Fog = new MapFog();
        public IList<MapNPC> NPC = Array.Empty<MapNPC>();
        public IList<MapLight> Light = Array.Empty<MapLight>();
        public byte Lighting = 100;
        public Map[] Link = new Map[(byte)Directions.Count];

        // Construtor
        public Map(Guid ID) : base(ID)
        {
            for (byte x = 0; x < Width; x++)
                for (byte y = 0; y < Height; y++)
                    Attribute[x, y] = new MapAttribute();
        }

        // Verifica se as coordenas estão no limite do mapa
        public bool OutLimit(byte X, byte Y) => X >= Width || Y >= Height || X < 0 || Y < 0;

        public bool Tile_Blocked(byte X, byte Y)
        {
            // Verifica se o azulejo está bloqueado
            if (OutLimit(X, Y)) return true;
            if (Attribute[X, Y].Type == (byte)TileAttributes.Block) return true;
            return false;
        }
    }

    [Serializable]
    public class MapAttribute
    {
        public byte Type;
        public string Data_1;
        public short Data_2;
        public short Data_3;
        public short Data_4;
        public byte Zone;
        public bool[] Block = new bool[(byte)Directions.Count];
    }

    [Serializable]
    public class MapLayer
    {
        public string Name;
        public byte Type;
        public MapTileData[,] Tile = new MapTileData[Map.Width, Map.Height];

        public MapLayer()
        {
            for (byte x = 0; x < Map.Width; x++)
                for (byte y = 0; y < Map.Height; y++)
                    Tile[x, y] = new MapTileData();
        }
    }

    [Serializable]
    public class MapNPC
    {
        public NPC NPC;
        public byte Zone;
        public bool Spawn;
        public byte X;
        public byte Y;
    }

    [Serializable]
    public class MapTileData
    {
        public byte X;
        public byte Y;
        public byte Texture;
        public bool IsAutotile;
    }

    [Serializable]
    public class MapLight
    {
        public byte X;
        public byte Y;
        public byte Width;
        public byte Height;

        public MapLight() { }

        public MapLight(Rectangle Rec)
        {
            // Define os dados da estrutura
            X = (byte)Rec.X;
            Y = (byte)Rec.Y;
            Width = (byte)Rec.Width;
            Height = (byte)Rec.Height;
        }

        public Rectangle Rec
        {
            get
            {
                return new Rectangle(X, Y, Width, Height);
            }
        }
    }

    [Serializable]
    public class MapWeather
    {
        public Weathers Type;
        public byte Intensity;
    }

    [Serializable]
    public class MapFog
    {
        public byte Texture;
        public sbyte Speed_X;
        public sbyte Speed_Y;
        public byte Alpha = 255;
    }

    public struct MapWeatherParticle
    {
        public bool Visible;
        public int x;
        public int y;
        public int Speed;
        public int Start;
        public bool Back;
    }
}