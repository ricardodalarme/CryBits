using System.Drawing;
using static CryBits.Client.Logic.Game;

namespace CryBits.Client.Logic
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
            for (byte x = 0; x < MapWidth; x++)
                for (byte y = 0; y < MapHeight; y++)
                    for (byte c = 0; c < (byte)Layers.Count; c++)
                        for (byte q = 0; q <= Mapper.Current.Data.Tile[x, y].Data.GetUpperBound(1); q++)
                            if (Mapper.Current.Data.Tile[x, y].Data[c, q].Automatic)
                                // Faz os cálculos para a autocriação
                                Calculate(x, y, q, c);
        }

        private static void Set(byte x, byte y, byte layerNum, byte layerType, byte part, string index)
        {
            Point position = new Point(0);

            // Posições exatas dos mini azulejos (16x16)
            switch (index)
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
            Entities.MapTileData data = Mapper.Current.Data.Tile[x, y].Data[layerType, layerNum];
            Mapper.Current.Data.Tile[x, y].Data[layerType, layerNum].Mini[part].X = data.X * Grid + position.X;
            Mapper.Current.Data.Tile[x, y].Data[layerType, layerNum].Mini[part].Y = data.Y * Grid + position.Y;
        }

        private static bool Check(int x1, int y1, int x2, int y2, byte layerNum, byte layerType)
        {
            Entities.MapTileData data1, data2;

            // Somente se necessário
            if (x2 < 0 || x2 >= MapWidth || y2 < 0 || y2 >= MapHeight) return true;

            // Dados
            data1 = Mapper.Current.Data.Tile[x1, y1].Data[layerType, layerNum];
            data2 = Mapper.Current.Data.Tile[x2, y2].Data[layerType, layerNum];

            // Verifica se são os mesmo azulejos
            if (!data2.Automatic) return false;
            if (data1.Tile != data2.Tile) return false;
            if (data1.X != data2.X) return false;
            if (data1.Y != data2.Y) return false;

            // Não há nada de errado
            return true;
        }

        private static void Calculate(byte x, byte y, byte layerNum, byte layerType)
        {
            // Calcula as quatros partes do azulejo
            Calculate_NW(x, y, layerNum, layerType);
            Calculate_NE(x, y, layerNum, layerType);
            Calculate_SW(x, y, layerNum, layerType);
            Calculate_SE(x, y, layerNum, layerType);
        }

        private static void Calculate_NW(byte x, byte y, byte layerNum, byte layerType)
        {
            bool[] tile = new bool[4];
            AddMode mode = AddMode.None;

            // Verifica se existe algo para modificar nos azulejos em volta (Norte, Oeste, Noroeste)
            if (Check(x, y, x - 1, y - 1, layerNum, layerType)) tile[1] = true;
            if (Check(x, y, x, y - 1, layerNum, layerType)) tile[2] = true;
            if (Check(x, y, x - 1, y, layerNum, layerType)) tile[3] = true;

            // Forma que será adicionado o mini azulejo
            if (!tile[2] && !tile[3]) mode = AddMode.Inside;
            if (!tile[2] && tile[3]) mode = AddMode.Horizontal;
            if (tile[2] && !tile[3]) mode = AddMode.Vertical;
            if (!tile[1] && tile[2] && tile[3]) mode = AddMode.Exterior;
            if (tile[1] && tile[2] && tile[3]) mode = AddMode.Fill;

            // Define o mini azulejo
            switch (mode)
            {
                case AddMode.Inside: Set(x, y, layerNum, layerType, 0, "e"); break;
                case AddMode.Exterior: Set(x, y, layerNum, layerType, 0, "a"); break;
                case AddMode.Horizontal: Set(x, y, layerNum, layerType, 0, "i"); break;
                case AddMode.Vertical: Set(x, y, layerNum, layerType, 0, "m"); break;
                case AddMode.Fill: Set(x, y, layerNum, layerType, 0, "q"); break;
            }
        }

        private static void Calculate_NE(byte x, byte y, byte layerNum, byte layerType)
        {
            bool[] tile = new bool[4];
            AddMode mode = AddMode.None;

            // Verifica se existe algo para modificar nos azulejos em volta (Norte, Oeste, Noroeste)
            if (Check(x, y, x, y - 1, layerNum, layerType)) tile[1] = true;
            if (Check(x, y, x + 1, y - 1, layerNum, layerType)) tile[2] = true;
            if (Check(x, y, x + 1, y, layerNum, layerType)) tile[3] = true;

            // Forma que será adicionado o mini azulejo
            if (!tile[1] && !tile[3]) mode = AddMode.Inside;
            if (!tile[1] && tile[3]) mode = AddMode.Horizontal;
            if (tile[1] && !tile[3]) mode = AddMode.Vertical;
            if (tile[1] && !tile[2] && tile[3]) mode = AddMode.Exterior;
            if (tile[1] && tile[2] && tile[3]) mode = AddMode.Fill;

            // Define o mini azulejo
            switch (mode)
            {
                case AddMode.Inside: Set(x, y, layerNum, layerType, 1, "j"); break;
                case AddMode.Exterior: Set(x, y, layerNum, layerType, 1, "b"); break;
                case AddMode.Horizontal: Set(x, y, layerNum, layerType, 1, "f"); break;
                case AddMode.Vertical: Set(x, y, layerNum, layerType, 1, "r"); break;
                case AddMode.Fill: Set(x, y, layerNum, layerType, 1, "n"); break;
            }
        }

        private static void Calculate_SW(byte x, byte y, byte layerNum, byte layerType)
        {
            bool[] tile = new bool[4];
            AddMode mode = AddMode.None;

            // Verifica se existe algo para modificar nos azulejos em volta (Sul, Oeste, Sudoeste)
            if (Check(x, y, x - 1, y, layerNum, layerType)) tile[1] = true;
            if (Check(x, y, x - 1, y + 1, layerNum, layerType)) tile[2] = true;
            if (Check(x, y, x, y + 1, layerNum, layerType)) tile[3] = true;

            // Forma que será adicionado o mini azulejo
            if (!tile[1] && !tile[3]) mode = AddMode.Inside;
            if (tile[1] && !tile[3]) mode = AddMode.Horizontal;
            if (!tile[1] && tile[3]) mode = AddMode.Vertical;
            if (tile[1] && !tile[2] && tile[3]) mode = AddMode.Exterior;
            if (tile[1] && tile[2] && tile[3]) mode = AddMode.Fill;

            // Define o mini azulejo
            switch (mode)
            {
                case AddMode.Inside: Set(x, y, layerNum, layerType, 2, "o"); break;
                case AddMode.Exterior: Set(x, y, layerNum, layerType, 2, "c"); break;
                case AddMode.Horizontal: Set(x, y, layerNum, layerType, 2, "s"); break;
                case AddMode.Vertical: Set(x, y, layerNum, layerType, 2, "g"); break;
                case AddMode.Fill: Set(x, y, layerNum, layerType, 2, "k"); break;
            }
        }

        private static void Calculate_SE(byte x, byte y, byte layerNum, byte layerType)
        {
            bool[] tile = new bool[4];
            AddMode mode = AddMode.None;

            // Verifica se existe algo para modificar nos azulejos em volta (Sul, Oeste, Sudeste)
            if (Check(x, y, x, y + 1, layerNum, layerType)) tile[1] = true;
            if (Check(x, y, x + 1, y + 1, layerNum, layerType)) tile[2] = true;
            if (Check(x, y, x + 1, y, layerNum, layerType)) tile[3] = true;

            // Forma que será adicionado o mini azulejo
            if (!tile[1] && !tile[3]) mode = AddMode.Inside;
            if (!tile[1] && tile[3]) mode = AddMode.Horizontal;
            if (tile[1] && !tile[3]) mode = AddMode.Vertical;
            if (tile[1] && !tile[2] && tile[3]) mode = AddMode.Exterior;
            if (tile[1] && tile[2] && tile[3]) mode = AddMode.Fill;

            // Define o mini azulejo
            switch (mode)
            {
                case AddMode.Inside: Set(x, y, layerNum, layerType, 3, "t"); break;
                case AddMode.Exterior: Set(x, y, layerNum, layerType, 3, "d"); break;
                case AddMode.Horizontal: Set(x, y, layerNum, layerType, 3, "p"); break;
                case AddMode.Vertical: Set(x, y, layerNum, layerType, 3, "l"); break;
                case AddMode.Fill: Set(x, y, layerNum, layerType, 3, "h"); break;
            }
        }
    }
}