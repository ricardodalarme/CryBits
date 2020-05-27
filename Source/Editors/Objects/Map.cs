using System;
using System.Collections.Generic;
using System.Drawing;

namespace Objects
{
    [Serializable]
    class Map : Lists.Structures.Data
    {
        // Dados gerais
        public short Revision;
        public List<Map_Layer> Layer = new List<Map_Layer>();
        public Map_Attribute[,] Attribute = new Map_Attribute[Globals.Map_Width, Globals.Map_Height];
        public string Name;
        public Globals.Map_Morals Moral;
        public byte Panorama;
        public Audio.Musics Music;
        public Color Color;
        public Map_Weather Weather = new Map_Weather();
        public Map_Fog Fog = new Map_Fog();
        public Map[] Link = new Map[(byte)Globals.Directions.Count];
        public byte Lighting;
        public List<Map_Light> Light = new List<Map_Light>();
        public List<Map_NPC> NPC = new List<Map_NPC>();

        // Construtor
        public Map(Guid ID) : base(ID)
        {
            for (byte x = 0; x < Globals.Map_Width; x++)
                for (byte y = 0; y < Globals.Map_Height; y++)
                    Attribute[x, y] = new Map_Attribute();
        }

        public override string ToString() => Name;

        // Verifica se as coordenas estão no limite do mapa
        public bool OutLimit(short x, short y) => x >= Globals.Map_Width || y >= Globals.Map_Height || x < 0 || y < 0;
    }

    class Map_Attribute
    {
        public byte Type;
        public string Data_1;
        public short Data_2;
        public short Data_3;
        public short Data_4;
        public byte Zone;
        public bool[] Block = new bool[(byte)Globals.Directions.Count];
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
        public Globals.Weathers Type;
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
        public Map_Tile_Data[,] Tile = new Map_Tile_Data[Globals.Map_Width, Globals.Map_Height];

        public Map_Layer()
        {
            for (byte x = 0; x < Globals.Map_Width; x++)
                for (byte y = 0; y < Globals.Map_Height; y++)
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