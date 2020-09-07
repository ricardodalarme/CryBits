using Logic;

namespace Entities
{
    class MapLayer
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
}
