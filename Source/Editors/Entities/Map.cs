using Editors;
using System;
using System.Collections.Generic;
using System.Drawing;
using static Logic.Utils;

namespace Entities
{
    [Serializable]
    class Map : Entity
    {
        // Lista de dados
        public static Dictionary<Guid, Map> List = new Dictionary<Guid, Map>();

        // Obtém o dado, caso ele não existir retorna nulo
        public static Map Get(Guid ID) => List.ContainsKey(ID) ? List[ID] : null;

        // Limitações dos mapas
        public const byte Width = 25;
        public const byte Height = 19;
        public const byte Num_Zones = 20;

        // Fumaças
        public static int Fog_X;
        public static int Fog_Y;

        // Clima
        public const byte Max_Rain_Particles = 100;
        public const short Max_Snow_Particles = 635;
        public const byte Max_Weather_Intensity = 10;
        public const byte Snow_Movement = 10;
        public static byte Lightning;

        // Dados gerais
        public short Revision;
        public List<Map_Layer> Layer = new List<Map_Layer>();
        public Map_Attribute[,] Attribute = new Map_Attribute[Width, Height];
        public string Name;
        public Map_Morals Moral;
        public byte Panorama;
        public Audio.Musics Music;
        public Color Color;
        public Map_Weather Weather = new Map_Weather();
        public Map_Fog Fog = new Map_Fog();
        public Map[] Link = new Map[(byte)Directions.Count];
        public byte Lighting;
        public List<Map_Light> Light = new List<Map_Light>();
        public List<Map_NPC> NPC = new List<Map_NPC>();

        // Construtor
        public Map(Guid ID) : base(ID)
        {
            for (byte x = 0; x < Width; x++)
                for (byte y = 0; y < Height; y++)
                    Attribute[x, y] = new Map_Attribute();
        }

        public override string ToString() => Name;

        // Verifica se as coordenas estão no limite do mapa
        public bool OutLimit(short x, short y) => x >= Width || y >= Height || x < 0 || y < 0;

        ///////////////////////
        // Funções Estáticas //
        ///////////////////////
        public static void Weather_Update()
        {
            // Redimensiona a lista
            if (Editor_Maps.Form != null)
                switch (Editor_Maps.Form.Selected.Weather.Type)
                {
                    case Weathers.Thundering:
                    case Weathers.Raining: Lists.Weather = new Lists.Structures.Weather[Max_Rain_Particles + 1]; break;
                    case Weathers.Snowing: Lists.Weather = new Lists.Structures.Weather[Max_Snow_Particles + 1]; break;
                }
        }
    }

    class Map_Attribute
    {
        public byte Type;
        public string Data_1;
        public short Data_2;
        public short Data_3;
        public short Data_4;
        public byte Zone;
        public bool[] Block = new bool[(byte)Directions.Count];
    }

    class Map_Light
    {
        public byte X;
        public byte Y;
        public byte Width;
        public byte Height;

        public Map_Light(Rectangle Rec)
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

    class Map_Weather
    {
        public Weathers Type;
        public byte Intensity;
    }

    class Map_Fog
    {
        public byte Texture;
        public sbyte Speed_X;
        public sbyte Speed_Y;
        public byte Alpha;
    }

    class Map_Tile_Data
    {
        public byte X;
        public byte Y;
        public byte Tile;
        public bool Auto;
        public Point[] Mini = new Point[4];
    }

    class Map_Layer
    {
        public string Name;
        public byte Type;
        public Map_Tile_Data[,] Tile = new Map_Tile_Data[Map.Width, Map.Height];

        public Map_Layer()
        {
            for (byte x = 0; x < Map.Width; x++)
                for (byte y = 0; y < Map.Height; y++)
                    Tile[x, y] = new Map_Tile_Data();
        }
    }

    class Map_NPC
    {
        public NPC NPC;
        public byte Zone;
        public bool Spawn;
        public byte X;
        public byte Y;
    }
}