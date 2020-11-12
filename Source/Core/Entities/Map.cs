using System;
using System.Collections.Generic;
using System.Drawing;
using static CryBits.Utils;

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

        // Quantidade de zonas
        public const byte Num_Zones = 20;

        // Clima
        public const byte Max_Rain_Particles = 100;
        public const short Max_Snow_Particles = 635;
        public const byte Max_Weather_Intensity = 10;
        public const byte Snow_Movement = 10;

        // Dados
        public short Revision { get; set; }
        public Map_Morals Moral { get; set; }
        public IList<MapLayer> Layer { get; set; } = new List<MapLayer>();
        public MapAttribute[,] Attribute = new MapAttribute[Width, Height];
        public byte Panorama { get; set; }
        public byte Music { get; set; }
        public Color Color { get; set; } = Color.FromArgb(-1);
        public MapWeather Weather = new MapWeather();
        public MapFog Fog = new MapFog();
        public IList<MapNPC> NPC { get; set; } = new List<MapNPC>();
        public IList<MapLight> Light { get; set; } = new List<MapLight>();
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
        public bool OutLimit(short X, short Y) => X >= Width || Y >= Height || X < 0 || Y < 0;

        public bool Tile_Blocked(short X, short Y)
        {
            // Verifica se o azulejo está bloqueado
            if (OutLimit(X, Y)) return true;
            if (Attribute[X, Y].Type == (byte)TileAttributes.Block) return true;
            return false;
        }

        public void Update()
        {
            // Atualiza os azulejos necessários
            for (byte x = 0; x < Width; x++)
                for (byte y = 0; y < Height; y++)
                    for (byte c = 0; c < Layer.Count; c++)
                        if (Layer[c].Tile[x, y].IsAutotile)
                            // Faz os cálculos para a autocriação
                            Layer[c].Calculate(x, y);
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

        public void Update(int x, int y)
        {
            // Atualiza os azulejos necessários
            for (int x2 = x - 2; x2 < x + 2; x2++)
                for (int y2 = y - 2; y2 < y + 2; y2++)
                    if (x2 >= 0 && x2 < Map.Width && y2 >= 0 && y2 < Map.Height)
                        // Faz os cálculos para a autocriação
                        Calculate((byte)x2, (byte)y2);
        }

        private bool Check(int X1, int Y1, int X2, int Y2)
        {
            MapTileData Data1, Data2;

            // Somente se necessário
            if (X1 < 0 || X1 >= Map.Width || Y1 < 0 || Y1 >= Map.Height) return true;
            if (X2 < 0 || X2 >= Map.Width || Y2 < 0 || Y2 >= Map.Height) return true;

            // Dados
            Data1 = Tile[X1, Y1];
            Data2 = Tile[X2, Y2];

            // Verifica se são os mesmo azulejos
            if (!Data2.IsAutotile) return false;
            if (Data1.Texture != Data2.Texture) return false;
            if (Data1.X != Data2.X) return false;
            if (Data1.Y != Data2.Y) return false;

            // Não há nada de errado
            return true;
        }

        public void Calculate(byte x, byte y)
        {
            // Calcula as quatros partes do azulejo
            CalculateNW(x, y);
            CalculateNE(x, y);
            CalculateSW(x, y);
            CalculateSE(x, y);
        }

        private void CalculateNW(byte x, byte y)
        {
            bool[] Avaliable = new bool[3];
            AddMode Mode = AddMode.None;

            // Verifica se existe algo para modificar nos azulejos em volta (Norte, Oeste, Noroeste)
            if (Check(x, y, x - 1, y - 1)) Avaliable[0] = true;
            if (Check(x, y, x, y - 1)) Avaliable[1] = true;
            if (Check(x, y, x - 1, y)) Avaliable[2] = true;

            // Forma que será adicionado o mini azulejo
            if (!Avaliable[1] && !Avaliable[2]) Mode = AddMode.Inside;
            if (!Avaliable[1] && Avaliable[2]) Mode = AddMode.Horizontal;
            if (Avaliable[1] && !Avaliable[2]) Mode = AddMode.Vertical;
            if (!Avaliable[0] && Avaliable[1] && Avaliable[2]) Mode = AddMode.Exterior;
            if (Avaliable[0] && Avaliable[1] && Avaliable[2]) Mode = AddMode.Fill;

            // Define o mini azulejo
            switch (Mode)
            {
                case AddMode.Inside: Tile[x, y].SetMini(0, "e"); break;
                case AddMode.Exterior: Tile[x, y].SetMini(0, "a"); break;
                case AddMode.Horizontal: Tile[x, y].SetMini(0, "i"); break;
                case AddMode.Vertical: Tile[x, y].SetMini(0, "m"); break;
                case AddMode.Fill: Tile[x, y].SetMini(0, "q"); break;
            }
        }

        private void CalculateNE(byte x, byte y)
        {
            bool[] Avaliable = new bool[3];
            AddMode Mode = AddMode.None;

            // Verifica se existe algo para modificar nos azulejos em volta (Norte, Oeste, Noroeste)
            if (Check(x, y, x, y - 1)) Avaliable[0] = true;
            if (Check(x, y, x + 1, y - 1)) Avaliable[1] = true;
            if (Check(x, y, x + 1, y)) Avaliable[2] = true;

            // Forma que será adicionado o mini azulejo
            if (!Avaliable[0] && !Avaliable[2]) Mode = AddMode.Inside;
            if (!Avaliable[0] && Avaliable[2]) Mode = AddMode.Horizontal;
            if (Avaliable[0] && !Avaliable[2]) Mode = AddMode.Vertical;
            if (Avaliable[0] && !Avaliable[1] && Avaliable[2]) Mode = AddMode.Exterior;
            if (Avaliable[0] && Avaliable[1] && Avaliable[2]) Mode = AddMode.Fill;

            // Define o mini azulejo
            switch (Mode)
            {
                case AddMode.Inside: Tile[x, y].SetMini(1, "j"); break;
                case AddMode.Exterior: Tile[x, y].SetMini(1, "b"); break;
                case AddMode.Horizontal: Tile[x, y].SetMini(1, "f"); break;
                case AddMode.Vertical: Tile[x, y].SetMini(1, "r"); break;
                case AddMode.Fill: Tile[x, y].SetMini(1, "n"); break;
            }
        }

        private void CalculateSW(byte x, byte y)
        {
            bool[] Avaliable = new bool[3];
            AddMode Mode = AddMode.None;

            // Verifica se existe algo para modificar nos azulejos em volta (Sul, Oeste, Sudoeste)
            if (Check(x, y, x - 1, y)) Avaliable[0] = true;
            if (Check(x, y, x - 1, y + 1)) Avaliable[1] = true;
            if (Check(x, y, x, y + 1)) Avaliable[2] = true;

            // Forma que será adicionado o mini azulejo
            if (!Avaliable[0] && !Avaliable[2]) Mode = AddMode.Inside;
            if (Avaliable[0] && !Avaliable[2]) Mode = AddMode.Horizontal;
            if (!Avaliable[0] && Avaliable[2]) Mode = AddMode.Vertical;
            if (Avaliable[0] && !Avaliable[1] && Avaliable[2]) Mode = AddMode.Exterior;
            if (Avaliable[0] && Avaliable[1] && Avaliable[2]) Mode = AddMode.Fill;

            // Define o mini azulejo
            switch (Mode)
            {
                case AddMode.Inside: Tile[x, y].SetMini(2, "o"); break;
                case AddMode.Exterior: Tile[x, y].SetMini(2, "c"); break;
                case AddMode.Horizontal: Tile[x, y].SetMini(2, "s"); break;
                case AddMode.Vertical: Tile[x, y].SetMini(2, "g"); break;
                case AddMode.Fill: Tile[x, y].SetMini(2, "k"); break;
            }
        }

        private void CalculateSE(byte x, byte y)
        {
            bool[] Avaliable = new bool[3];
            AddMode Mode = AddMode.None;

            // Verifica se existe algo para modificar nos azulejos em volta (Sul, Oeste, Sudeste)
            if (Check(x, y, x, y + 1)) Avaliable[0] = true;
            if (Check(x, y, x + 1, y + 1)) Avaliable[1] = true;
            if (Check(x, y, x + 1, y)) Avaliable[2] = true;

            // Forma que será adicionado o mini azulejo
            if (!Avaliable[0] && !Avaliable[2]) Mode = AddMode.Inside;
            if (!Avaliable[0] && Avaliable[2]) Mode = AddMode.Horizontal;
            if (Avaliable[0] && !Avaliable[2]) Mode = AddMode.Vertical;
            if (Avaliable[0] && !Avaliable[1] && Avaliable[2]) Mode = AddMode.Exterior;
            if (Avaliable[0] && Avaliable[1] && Avaliable[2]) Mode = AddMode.Fill;

            // Define o mini azulejo
            switch (Mode)
            {
                case AddMode.Inside: Tile[x, y].SetMini(3, "t"); break;
                case AddMode.Exterior: Tile[x, y].SetMini(3, "d"); break;
                case AddMode.Horizontal: Tile[x, y].SetMini(3, "p"); break;
                case AddMode.Vertical: Tile[x, y].SetMini(3, "l"); break;
                case AddMode.Fill: Tile[x, y].SetMini(3, "h"); break;
            }
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
        public Point[] Mini = new Point[4];

        public void SetMini(byte Index, string Mode)
        {
            Point Position = new Point(0);

            // Posições exatas dos mini azulejos (16x16)
            switch (Mode)
            {
                // Quinas
                case "a": Position = new Point(32, 0); break;
                case "b": Position = new Point(48, 0); break;
                case "c": Position = new Point(32, 16); break;
                case "d": Position = new Point(48, 16); break;

                // Noroeste
                case "e": Position = new Point(0, 32); break;
                case "f": Position = new Point(16, 32); break;
                case "g": Position = new Point(0, 48); break;
                case "h": Position = new Point(16, 48); break;

                // Nordeste
                case "i": Position = new Point(32, 32); break;
                case "j": Position = new Point(48, 32); break;
                case "k": Position = new Point(32, 48); break;
                case "l": Position = new Point(48, 48); break;

                // Sudoeste
                case "m": Position = new Point(0, 64); break;
                case "n": Position = new Point(16, 64); break;
                case "o": Position = new Point(0, 80); break;
                case "p": Position = new Point(16, 80); break;

                // Sudeste
                case "q": Position = new Point(32, 64); break;
                case "r": Position = new Point(48, 64); break;
                case "s": Position = new Point(32, 80); break;
                case "t": Position = new Point(48, 80); break;
            }

            // Define a posição do mini azulejo
            Mini[Index].X = X * Grid + Position.X;
            Mini[Index].Y = Y * Grid + Position.Y;
        }
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