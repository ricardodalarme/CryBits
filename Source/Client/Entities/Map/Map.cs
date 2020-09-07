using System;
using System.Collections.Generic;
using Logic;
using static Logic.Game;

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


        private static bool Check(int X1, int Y1, int X2, int Y2, byte Layer_Num, byte Layer_Type)
        {
            Entities.MapTileData Data1, Data2;

            // Somente se necessário
            if (X2 < 0 || X2 >= Map_Width || Y2 < 0 || Y2 >= Map_Height) return true;

            // Dados
            Data1 = Mapper.Current.Data.Tile[X1, Y1].Data[Layer_Type, Layer_Num];
            Data2 = Mapper.Current.Data.Tile[X2, Y2].Data[Layer_Type, Layer_Num];

            // Verifica se são os mesmo azulejos
            if (!Data2.IsAutotile) return false;
            if (Data1.Texture != Data2.Texture) return false;
            if (Data1.X != Data2.X) return false;
            if (Data1.Y != Data2.Y) return false;

            // Não há nada de errado
            return true;
        }

        private static void Calculate(byte x, byte y, byte Layer_Num, byte Layer_Type)
        {
            // Calcula as quatros partes do azulejo
            CalculateNW(x, y, Layer_Num, Layer_Type);
            CalculateNE(x, y, Layer_Num, Layer_Type);
            CalculateSW(x, y, Layer_Num, Layer_Type);
            CalculateSE(x, y, Layer_Num, Layer_Type);
        }

        private static void CalculateNW(byte x, byte y, byte Layer_Num, byte Layer_Type)
        {
            bool[] Tile = new bool[4];
            AddMode Mode = AddMode.None;

            // Verifica se existe algo para modificar nos azulejos em volta (Norte, Oeste, Noroeste)
            if (Check(x, y, x - 1, y - 1, Layer_Num, Layer_Type)) Tile[1] = true;
            if (Check(x, y, x, y - 1, Layer_Num, Layer_Type)) Tile[2] = true;
            if (Check(x, y, x - 1, y, Layer_Num, Layer_Type)) Tile[3] = true;

            // Forma que será adicionado o mini azulejo
            if (!Tile[2] && !Tile[3]) Mode = AddMode.Inside;
            if (!Tile[2] && Tile[3]) Mode = AddMode.Horizontal;
            if (Tile[2] && !Tile[3]) Mode = AddMode.Vertical;
            if (!Tile[1] && Tile[2] && Tile[3]) Mode = AddMode.Exterior;
            if (Tile[1] && Tile[2] && Tile[3]) Mode = AddMode.Fill;

            // Define o mini azulejo
            switch (Mode)
            {
                case AddMode.Inside: Set(x, y, Layer_Num, Layer_Type, 0, "e"); break;
                case AddMode.Exterior: Set(x, y, Layer_Num, Layer_Type, 0, "a"); break;
                case AddMode.Horizontal: Set(x, y, Layer_Num, Layer_Type, 0, "i"); break;
                case AddMode.Vertical: Set(x, y, Layer_Num, Layer_Type, 0, "m"); break;
                case AddMode.Fill: Set(x, y, Layer_Num, Layer_Type, 0, "q"); break;
            }
        }

        private static void CalculateNE(byte x, byte y, byte Layer_Num, byte Layer_Type)
        {
            bool[] Tile = new bool[4];
            AddMode Mode = AddMode.None;

            // Verifica se existe algo para modificar nos azulejos em volta (Norte, Oeste, Noroeste)
            if (Check(x, y, x, y - 1, Layer_Num, Layer_Type)) Tile[1] = true;
            if (Check(x, y, x + 1, y - 1, Layer_Num, Layer_Type)) Tile[2] = true;
            if (Check(x, y, x + 1, y, Layer_Num, Layer_Type)) Tile[3] = true;

            // Forma que será adicionado o mini azulejo
            if (!Tile[1] && !Tile[3]) Mode = AddMode.Inside;
            if (!Tile[1] && Tile[3]) Mode = AddMode.Horizontal;
            if (Tile[1] && !Tile[3]) Mode = AddMode.Vertical;
            if (Tile[1] && !Tile[2] && Tile[3]) Mode = AddMode.Exterior;
            if (Tile[1] && Tile[2] && Tile[3]) Mode = AddMode.Fill;

            // Define o mini azulejo
            switch (Mode)
            {
                case AddMode.Inside: Set(x, y, Layer_Num, Layer_Type, 1, "j"); break;
                case AddMode.Exterior: Set(x, y, Layer_Num, Layer_Type, 1, "b"); break;
                case AddMode.Horizontal: Set(x, y, Layer_Num, Layer_Type, 1, "f"); break;
                case AddMode.Vertical: Set(x, y, Layer_Num, Layer_Type, 1, "r"); break;
                case AddMode.Fill: Set(x, y, Layer_Num, Layer_Type, 1, "n"); break;
            }
        }

        private static void CalculateSW(byte x, byte y, byte Layer_Num, byte Layer_Type)
        {
            bool[] Tile = new bool[4];
            AddMode Mode = AddMode.None;

            // Verifica se existe algo para modificar nos azulejos em volta (Sul, Oeste, Sudoeste)
            if (Check(x, y, x - 1, y, Layer_Num, Layer_Type)) Tile[1] = true;
            if (Check(x, y, x - 1, y + 1, Layer_Num, Layer_Type)) Tile[2] = true;
            if (Check(x, y, x, y + 1, Layer_Num, Layer_Type)) Tile[3] = true;

            // Forma que será adicionado o mini azulejo
            if (!Tile[1] && !Tile[3]) Mode = AddMode.Inside;
            if (Tile[1] && !Tile[3]) Mode = AddMode.Horizontal;
            if (!Tile[1] && Tile[3]) Mode = AddMode.Vertical;
            if (Tile[1] && !Tile[2] && Tile[3]) Mode = AddMode.Exterior;
            if (Tile[1] && Tile[2] && Tile[3]) Mode = AddMode.Fill;

            // Define o mini azulejo
            switch (Mode)
            {
                case AddMode.Inside: Set(x, y, Layer_Num, Layer_Type, 2, "o"); break;
                case AddMode.Exterior: Set(x, y, Layer_Num, Layer_Type, 2, "c"); break;
                case AddMode.Horizontal: Set(x, y, Layer_Num, Layer_Type, 2, "s"); break;
                case AddMode.Vertical: Set(x, y, Layer_Num, Layer_Type, 2, "g"); break;
                case AddMode.Fill: Set(x, y, Layer_Num, Layer_Type, 2, "k"); break;
            }
        }

        private static void CalculateSE(byte x, byte y, byte Layer_Num, byte Layer_Type)
        {
            bool[] Tile = new bool[4];
            AddMode Mode = AddMode.None;

            // Verifica se existe algo para modificar nos azulejos em volta (Sul, Oeste, Sudeste)
            if (Check(x, y, x, y + 1, Layer_Num, Layer_Type)) Tile[1] = true;
            if (Check(x, y, x + 1, y + 1, Layer_Num, Layer_Type)) Tile[2] = true;
            if (Check(x, y, x + 1, y, Layer_Num, Layer_Type)) Tile[3] = true;

            // Forma que será adicionado o mini azulejo
            if (!Tile[1] && !Tile[3]) Mode = AddMode.Inside;
            if (!Tile[1] && Tile[3]) Mode = AddMode.Horizontal;
            if (Tile[1] && !Tile[3]) Mode = AddMode.Vertical;
            if (Tile[1] && !Tile[2] && Tile[3]) Mode = AddMode.Exterior;
            if (Tile[1] && Tile[2] && Tile[3]) Mode = AddMode.Fill;

            // Define o mini azulejo
            switch (Mode)
            {
                case AddMode.Inside: Set(x, y, Layer_Num, Layer_Type, 3, "t"); break;
                case AddMode.Exterior: Set(x, y, Layer_Num, Layer_Type, 3, "d"); break;
                case AddMode.Horizontal: Set(x, y, Layer_Num, Layer_Type, 3, "p"); break;
                case AddMode.Vertical: Set(x, y, Layer_Num, Layer_Type, 3, "l"); break;
                case AddMode.Fill: Set(x, y, Layer_Num, Layer_Type, 3, "h"); break;
            }
        }
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
        public MapTileData[,] Data;
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