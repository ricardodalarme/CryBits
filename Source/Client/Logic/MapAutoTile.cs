using System.Drawing;
using static Logic.Utils;

namespace Logic
{
    class MapAutoTile
    {   // Formas de adicionar o mini azulejo
        public enum AddMode
        {
            None,
            Inside,
            Exterior,
            Horizontal,
            Vertical,
            Fill
        }

        public static void Update()
        {
            // Atualiza os azulejos necessários
            for (byte x = 0; x < Map_Width; x++)
                for (byte y = 0; y < Map_Height; y++)
                    for (byte c = 0; c < (byte)Mapper.Layers.Count; c++)
                        for (byte q = 0; q <= Mapper.Current.Data.Tile[x, y].Data.GetUpperBound(1); q++)
                            if (Mapper.Current.Data.Tile[x, y].Data[c, q].Automatic)
                                // Faz os cálculos para a autocriação
                                Calculate(x, y, q, c);
        }

        private static void Set(byte x, byte y, byte Layer_Num, byte Layer_Type, byte Part, string Index)
        {
            Point Position = new Point(0);

            // Posições exatas dos mini azulejos (16x16)
            switch (Index)
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
            Entities.Map_Tile_Data Data = Mapper.Current.Data.Tile[x, y].Data[Layer_Type, Layer_Num];
            Mapper.Current.Data.Tile[x, y].Data[Layer_Type, Layer_Num].Mini[Part].X = Data.X * Grid + Position.X;
            Mapper.Current.Data.Tile[x, y].Data[Layer_Type, Layer_Num].Mini[Part].Y = Data.Y * Grid + Position.Y;
        }

        private static bool Check(int X1, int Y1, int X2, int Y2, byte Layer_Num, byte Layer_Type)
        {
            Entities.Map_Tile_Data Data1, Data2;

            // Somente se necessário
            if (X2 < 0 || X2 >= Map_Width || Y2 < 0 || Y2 >= Map_Height) return true;

            // Dados
            Data1 = Mapper.Current.Data.Tile[X1, Y1].Data[Layer_Type, Layer_Num];
            Data2 = Mapper.Current.Data.Tile[X2, Y2].Data[Layer_Type, Layer_Num];

            // Verifica se são os mesmo azulejos
            if (!Data2.Automatic) return false;
            if (Data1.Tile != Data2.Tile) return false;
            if (Data1.X != Data2.X) return false;
            if (Data1.Y != Data2.Y) return false;

            // Não há nada de errado
            return true;
        }

        private static void Calculate(byte x, byte y, byte Layer_Num, byte Layer_Type)
        {
            // Calcula as quatros partes do azulejo
            Calculate_NW(x, y, Layer_Num, Layer_Type);
            Calculate_NE(x, y, Layer_Num, Layer_Type);
            Calculate_SW(x, y, Layer_Num, Layer_Type);
            Calculate_SE(x, y, Layer_Num, Layer_Type);
        }

        private static void Calculate_NW(byte x, byte y, byte Layer_Num, byte Layer_Type)
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

        private static void Calculate_NE(byte x, byte y, byte Layer_Num, byte Layer_Type)
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

        private static void Calculate_SW(byte x, byte y, byte Layer_Num, byte Layer_Type)
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

        private static void Calculate_SE(byte x, byte y, byte Layer_Num, byte Layer_Type)
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
}