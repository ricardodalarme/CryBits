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
        public static Map Get(Guid id) => List.ContainsKey(id) ? List[id] : null;

        // Tamanho dos mapas
        public const byte Width = 25;
        public const byte Height = 19;

        // Quantidade de zonas
        public const byte NumZones = 20;

        // Clima
        public const byte MaxRainParticles = 100;
        public const short MaxSnowParticles = 635;
        public const byte MaxWeatherIntensity = 10;
        public const byte SnowMovement = 10;

        // Dados
        public short Revision { get; set; }
        public Morals Moral { get; set; }
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
        public Map(Guid id) : base(id)
        {
            for (byte x = 0; x < Width; x++)
                for (byte y = 0; y < Height; y++)
                    Attribute[x, y] = new MapAttribute();
        }

        // Verifica se as coordenas estão no limite do mapa
        public bool OutLimit(short x, short y) => x >= Width || y >= Height || x < 0 || y < 0;

        public bool Tile_Blocked(short x, short y)
        {
            // Verifica se o azulejo está bloqueado
            if (OutLimit(x, y)) return true;
            if (Attribute[x, y].Type == (byte)TileAttributes.Block) return true;
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

        private bool Check(int x1, int y1, int x2, int y2)
        {
            MapTileData data1, data2;

            // Somente se necessário
            if (x1 < 0 || x1 >= Map.Width || y1 < 0 || y1 >= Map.Height) return true;
            if (x2 < 0 || x2 >= Map.Width || y2 < 0 || y2 >= Map.Height) return true;

            // Dados
            data1 = Tile[x1, y1];
            data2 = Tile[x2, y2];

            // Verifica se são os mesmo azulejos
            if (!data2.IsAutotile) return false;
            if (data1.Texture != data2.Texture) return false;
            if (data1.X != data2.X) return false;
            if (data1.Y != data2.Y) return false;

            // Não há nada de errado
            return true;
        }

        public void Calculate(byte x, byte y)
        {
            // Calcula as quatros partes do azulejo
            CalculateNw(x, y);
            CalculateNe(x, y);
            CalculateSw(x, y);
            CalculateSe(x, y);
        }

        private void CalculateNw(byte x, byte y)
        {
            bool[] avaliable = new bool[3];
            AddMode mode = AddMode.None;

            // Verifica se existe algo para modificar nos azulejos em volta (Norte, Oeste, Noroeste)
            if (Check(x, y, x - 1, y - 1)) avaliable[0] = true;
            if (Check(x, y, x, y - 1)) avaliable[1] = true;
            if (Check(x, y, x - 1, y)) avaliable[2] = true;

            // Forma que será adicionado o mini azulejo
            if (!avaliable[1] && !avaliable[2]) mode = AddMode.Inside;
            if (!avaliable[1] && avaliable[2]) mode = AddMode.Horizontal;
            if (avaliable[1] && !avaliable[2]) mode = AddMode.Vertical;
            if (!avaliable[0] && avaliable[1] && avaliable[2]) mode = AddMode.Exterior;
            if (avaliable[0] && avaliable[1] && avaliable[2]) mode = AddMode.Fill;

            // Define o mini azulejo
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
            bool[] avaliable = new bool[3];
            AddMode mode = AddMode.None;

            // Verifica se existe algo para modificar nos azulejos em volta (Norte, Oeste, Noroeste)
            if (Check(x, y, x, y - 1)) avaliable[0] = true;
            if (Check(x, y, x + 1, y - 1)) avaliable[1] = true;
            if (Check(x, y, x + 1, y)) avaliable[2] = true;

            // Forma que será adicionado o mini azulejo
            if (!avaliable[0] && !avaliable[2]) mode = AddMode.Inside;
            if (!avaliable[0] && avaliable[2]) mode = AddMode.Horizontal;
            if (avaliable[0] && !avaliable[2]) mode = AddMode.Vertical;
            if (avaliable[0] && !avaliable[1] && avaliable[2]) mode = AddMode.Exterior;
            if (avaliable[0] && avaliable[1] && avaliable[2]) mode = AddMode.Fill;

            // Define o mini azulejo
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
            bool[] avaliable = new bool[3];
            AddMode mode = AddMode.None;

            // Verifica se existe algo para modificar nos azulejos em volta (Sul, Oeste, Sudoeste)
            if (Check(x, y, x - 1, y)) avaliable[0] = true;
            if (Check(x, y, x - 1, y + 1)) avaliable[1] = true;
            if (Check(x, y, x, y + 1)) avaliable[2] = true;

            // Forma que será adicionado o mini azulejo
            if (!avaliable[0] && !avaliable[2]) mode = AddMode.Inside;
            if (avaliable[0] && !avaliable[2]) mode = AddMode.Horizontal;
            if (!avaliable[0] && avaliable[2]) mode = AddMode.Vertical;
            if (avaliable[0] && !avaliable[1] && avaliable[2]) mode = AddMode.Exterior;
            if (avaliable[0] && avaliable[1] && avaliable[2]) mode = AddMode.Fill;

            // Define o mini azulejo
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
            bool[] avaliable = new bool[3];
            AddMode mode = AddMode.None;

            // Verifica se existe algo para modificar nos azulejos em volta (Sul, Oeste, Sudeste)
            if (Check(x, y, x, y + 1)) avaliable[0] = true;
            if (Check(x, y, x + 1, y + 1)) avaliable[1] = true;
            if (Check(x, y, x + 1, y)) avaliable[2] = true;

            // Forma que será adicionado o mini azulejo
            if (!avaliable[0] && !avaliable[2]) mode = AddMode.Inside;
            if (!avaliable[0] && avaliable[2]) mode = AddMode.Horizontal;
            if (avaliable[0] && !avaliable[2]) mode = AddMode.Vertical;
            if (avaliable[0] && !avaliable[1] && avaliable[2]) mode = AddMode.Exterior;
            if (avaliable[0] && avaliable[1] && avaliable[2]) mode = AddMode.Fill;

            // Define o mini azulejo
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

        public void SetMini(byte index, string mode)
        {
            Point position = new Point(0);

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
            Mini[index].X = X * Grid + position.X;
            Mini[index].Y = Y * Grid + position.Y;
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

        public MapLight(Rectangle rec)
        {
            // Define os dados da estrutura
            X = (byte)rec.X;
            Y = (byte)rec.Y;
            Width = (byte)rec.Width;
            Height = (byte)rec.Height;
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
        public int X;
        public int Y;
        public int Speed;
        public int Start;
        public bool Back;
    }
}