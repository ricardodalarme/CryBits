using Objects;
using System;
using System.Drawing;

class Mapper
{
    // Mapa atual
    public static Objects.TMap Current;

    // Fumaças
    public static int Fog_X;
    public static int Fog_Y;
    private static int Fog_X_Timer = 0;
    private static int Fog_Y_Timer = 0;

    // Clima
    public const byte Max_Rain = 100;
    public const short Max_Snow = 635;
    public const byte Max_Weather_Intensity = 10;
    public const byte Snow_Movement = 10;
    public static byte Lightning;
    private static int Snow_Timer = 0;
    private static int Lightning_Timer = 0;

    // Sangue
    private static int Blood_Timer;

    ////////////////
    // Numerações //
    ////////////////
    public enum Layers
    {
        Ground,
        Fringe,
        Count
    }

    public enum Layer_Attributes
    {
        None,
        Block,
        Warp,
        Count
    }

    public enum Weathers
    {
        Normal,
        Raining,
        Thundering,
        Snowing,
        Count
    }

    public enum Morals
    {
        Pacific,
        Danger,
        Count
    }

    public static void Logic()
    {
        // Toda a lógica do mapa
        Fog();
        Weather();

        // Retira os sangues do chão depois de um determinado tempo
        if (Blood_Timer < Environment.TickCount)
            for (byte i = 0; i < Current.Blood.Count; i++)
            {
                Current.Blood[i].Opacity -= 1;
                if (Current.Blood[i].Opacity == 0) Current.Blood.RemoveAt(i);
                Blood_Timer = Environment.TickCount + 100;
            }
    }

    public static void NextTile(Game.Directions Direction, ref byte X, ref byte Y)
    {
        // Próximo azulejo
        switch (Direction)
        {
            case Game.Directions.Up: Y -= 1; break;
            case Game.Directions.Down: Y += 1; break;
            case Game.Directions.Right: X += 1; break;
            case Game.Directions.Left: X -= 1; break;
        }
    }

    // Verifica se as coordenas estão no limite do mapa
    public static bool OutOfLimit(int x, int y) => x >= Game.Map_Width || y >= Game.Map_Height || x < 0 || y < 0;

    private static void Fog()
    {
        // Faz a movimentação
        Calculate_Fog_X();
        Calculate_Fog_Y();
    }

    private static void Calculate_Fog_X()
    {
        Size Size = Graphics.TSize(Graphics.Tex_Fog[Current.Data.Fog.Texture]);
        int Speed_X = Current.Data.Fog.Speed_X;

        // Apenas se necessário
        if (Fog_X_Timer >= Environment.TickCount) return;
        if (Speed_X == 0) return;

        // Movimento para trás
        if (Speed_X < 0)
        {
            Fog_X -= 1;
            if (Fog_X < -Size.Width) Fog_X = 0;
        }
        // Movimento para frente
        else
        {
            Fog_X += 1;
            if (Fog_X > Size.Width) Fog_X = 0;
        }

        // Contagem
        if (Speed_X < 0) Speed_X *= -1;
        Fog_X_Timer = Environment.TickCount + 50 - Speed_X;
    }

    private static void Calculate_Fog_Y()
    {
        Size Size = Graphics.TSize(Graphics.Tex_Fog[Current.Data.Fog.Texture]);
        int Speed_Y = Current.Data.Fog.Speed_Y;

        // Apenas se necessário
        if (Fog_Y_Timer >= Environment.TickCount) return;
        if (Speed_Y == 0) return;

        // Movimento para trás
        if (Speed_Y < 0)
        {
            Fog_Y -= 1;
            if (Fog_Y < -Size.Height) Fog_Y = 0;
        }
        // Movimento para frente
        else
        {
            Fog_Y += 1;
            if (Fog_Y > Size.Height) Fog_Y = 0;
        }

        // Contagem
        if (Speed_Y < 0) Speed_Y *= -1;
        Fog_Y_Timer = Environment.TickCount + 50 - Speed_Y;
    }

    private static void Weather()
    {
        bool Stop = false, Move;
        byte Thunder_First = (byte)Audio.Sounds.Thunder_1;
        byte Thunder_Last = (byte)Audio.Sounds.Thunder_4;

        // Somente se necessário
        if (Current.Data.Weather.Type == 0) return;

        // Contagem da neve
        if (Snow_Timer < Environment.TickCount)
        {
            Move = true;
            Snow_Timer = Environment.TickCount + 35;
        }
        else
            Move = false;

        // Contagem dos relâmpagos
        if (Lightning > 0)
            if (Lightning_Timer < Environment.TickCount)
            {
                Lightning -= 10;
                Lightning_Timer = Environment.TickCount + 25;
            }

        // Adiciona uma nova partícula
        for (short i = 1; i < TMap.Weather.Length; i++)
            if (!TMap.Weather[i].Visible)
            {
                if (Game.Random.Next(0, Max_Weather_Intensity - Current.Data.Weather.Intensity) == 0)
                {
                    if (!Stop)
                    {
                        // Cria a partícula
                        TMap.Weather[i].Visible = true;

                        // Cria a partícula de acordo com o seu tipo
                        switch ((Weathers)Current.Data.Weather.Type)
                        {
                            case Weathers.Thundering:
                            case Weathers.Raining: Weather_Rain_Create(i); break;
                            case Weathers.Snowing: Weather_Snow_Create(i); break;
                        }
                    }
                }

                Stop = true;
            }
            else
            {
                // Movimenta a partícula de acordo com o seu tipo
                switch ((Weathers)Current.Data.Weather.Type)
                {
                    case Weathers.Thundering:
                    case Weathers.Raining: Weather_Rain_Movement(i); break;
                    case Weathers.Snowing: Weather_Snow_Movement(i, Move); break;
                }

                // Reseta a partícula
                if (TMap.Weather[i].x > Game.Screen_Width || TMap.Weather[i].y > Game.Screen_Height)
                    TMap.Weather[i] = new TMap_Weather();
            }

        // Trovoadas
        if (Current.Data.Weather.Type == (byte)Weathers.Thundering)
            if (Game.Random.Next(0, Max_Weather_Intensity * 10 - Current.Data.Weather.Intensity * 2) == 0)
            {
                // Som do trovão
                int Thunder = Game.Random.Next(Thunder_First, Thunder_Last);
                Audio.Sound.Play((Audio.Sounds)Thunder);

                // Relâmpago
                if (Thunder < 6) Lightning = 190;
            }
    }

    private static void Weather_Rain_Create(int i)
    {
        // Define a velocidade e a posição da partícula
        TMap.Weather[i].Speed = Game.Random.Next(8, 13);

        if (Game.Random.Next(2) == 0)
        {
            TMap.Weather[i].x = -32;
            TMap.Weather[i].y = Game.Random.Next(-32, Game.Screen_Height);
        }
        else
        {
            TMap.Weather[i].x = Game.Random.Next(-32, Game.Screen_Width);
            TMap.Weather[i].y = -32;
        }
    }

    private static void Weather_Rain_Movement(int i)
    {
        // Movimenta a partícula
        TMap.Weather[i].x += TMap.Weather[i].Speed;
        TMap.Weather[i].y += TMap.Weather[i].Speed;
    }

    private static void Weather_Snow_Create(int i)
    {
        // Define a velocidade e a posição da partícula
        TMap.Weather[i].Speed = Game.Random.Next(1, 3);
        TMap.Weather[i].y = -32;
        TMap.Weather[i].x = Game.Random.Next(-32, Game.Screen_Width);
        TMap.Weather[i].Start = TMap.Weather[i].x;

        if (Game.Random.Next(2) == 0)
            TMap.Weather[i].Back = false;
        else
            TMap.Weather[i].Back = true;
    }

    private static void Weather_Snow_Movement(int i, bool Movimentrar = true)
    {
        int Diference = Game.Random.Next(0, Snow_Movement / 3);
        int x1 = TMap.Weather[i].Start + Snow_Movement + Diference;
        int x2 = TMap.Weather[i].Start - Snow_Movement - Diference;

        // Faz com que a partícula volte
        if (x1 <= TMap.Weather[i].x)
            TMap.Weather[i].Back = true;
        else if (x2 >= TMap.Weather[i].x)
            TMap.Weather[i].Back = false;

        // Movimenta a partícula
        TMap.Weather[i].y += TMap.Weather[i].Speed;

        if (Movimentrar)
            if (TMap.Weather[i].Back)
                TMap.Weather[i].x -= 1;
            else
                TMap.Weather[i].x += 1;
    }

    public static void Weather_Update()
    {
        // Para todos os sons
        Audio.Sound.Stop_All();

        // Redimensiona a lista
        switch ((Weathers)Current.Data.Weather.Type)
        {
            case Weathers.Thundering:
            case Weathers.Raining:
                // Reproduz o som chuva
                Audio.Sound.Play(Audio.Sounds.Rain, true);

                // Redimensiona a estrutura
                TMap.Weather = new TMap_Weather[Max_Rain + 1];
                break;
            case Weathers.Snowing: TMap.Weather = new TMap_Weather[Max_Snow + 1]; break;
        }
    }

    //////////////
    // Autotile //
    //////////////
    public class Autotile
    {
        // Formas de adicionar o mini azulejo
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
            for (byte x = 0; x < Game.Map_Width; x++)
                for (byte y = 0; y < Game.Map_Height; y++)
                    for (byte c = 0; c < (byte)Layers.Count; c++)
                        for (byte q = 0; q <= Current.Data.Tile[x, y].Data.GetUpperBound(1); q++)
                            if (Current.Data.Tile[x, y].Data[c, q].Automatic)
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
            Objects.Map_Tile_Data Data = Current.Data.Tile[x, y].Data[Layer_Type, Layer_Num];
            Current.Data.Tile[x, y].Data[Layer_Type, Layer_Num].Mini[Part].X = Data.X * Game.Grid + Position.X;
            Current.Data.Tile[x, y].Data[Layer_Type, Layer_Num].Mini[Part].Y = Data.Y * Game.Grid + Position.Y;
        }

        private static bool Check(int X1, int Y1, int X2, int Y2, byte Layer_Num, byte Layer_Type)
        {
            Objects.Map_Tile_Data Data1, Data2;

            // Somente se necessário
            if (X2 < 0 || X2 >= Game.Map_Width || Y2 < 0 || Y2 >= Game.Map_Height) return true;

            // Dados
            Data1 = Current.Data.Tile[X1, Y1].Data[Layer_Type, Layer_Num];
            Data2 = Current.Data.Tile[X2, Y2].Data[Layer_Type, Layer_Num];

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