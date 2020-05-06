using System;
using System.Drawing;

class Map
{
    // Limitações dos mapas
    public const byte Min_Width = 25;
    public const byte Min_Height = 19;

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
        Amount
    }

    public enum Layer_Attributes
    {
        None,
        Block,
        Warp,
        Amount
    }

    public enum Weathers
    {
        Normal,
        Raining,
        Thundering,
        Snowing,
        Amount
    }

    public enum Morals
    {
        Pacific,
        Danger,
        Amount
    }

    public static void Logic()
    {
        // Toda a lógica do mapa
        Fog();
        Weather();

        // Retira os sangues do chão depois de um determinado tempo
        if (Blood_Timer < Environment.TickCount)
            for (short i = 0; i < Lists.Temp_Map.Blood.Count; i++)
            {
                Lists.Temp_Map.Blood[i].Opacity -= 1;
                if (Lists.Temp_Map.Blood[i].Opacity == 0) Lists.Temp_Map.Blood.RemoveAt(i);
                Blood_Timer = Environment.TickCount + 100;
            }
    }

    private static void NextTile(Game.Directions Direction, ref short X, ref short Y)
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

    public static bool OutOfLimit(short x, short y)
    {
        // Verifica se as coordenas estão no limite do mapa
        return x > Lists.Map.Width || y > Lists.Map.Height || x < 0 || y < 0;
    }

    public static bool Tile_Blocked(short Map, byte X, byte Y, Game.Directions Direction)
    {
        short Next_X = X, Next_Y = Y;

        // Próximo azulejo
        NextTile(Direction, ref Next_X, ref Next_Y);

        // Verifica se está indo para uma ligação
        if (OutOfLimit(Next_X, Next_Y)) return Lists.Map.Link[(byte)Direction] == 0;

        // Verifica se o azulejo está bloqueado
        if (Lists.Map.Tile[Next_X, Next_Y].Attribute == (byte)Layer_Attributes.Block) return true;
        if (Lists.Map.Tile[Next_X, Next_Y].Block[(byte)Game.ReverseDirection(Direction)]) return true;
        if (Lists.Map.Tile[X, Y].Block[(byte)Direction]) return true;
        if (HasPlayer(Map, Next_X, Next_Y) != null || HasNPC(Next_X, Next_Y) > 0) return true;
        return false;
    }

    private static byte HasNPC(short X, short Y)
    {
        // Verifica se há algum npc na cordenada
        for (byte i = 1; i < Lists.Temp_Map.NPC.Length; i++)
            if (Lists.Temp_Map.NPC[i].Index > 0)
                if (Lists.Temp_Map.NPC[i].X == X && Lists.Temp_Map.NPC[i].Y == Y)
                    return i;

        return 0;
    }

    private static Player.Structure HasPlayer(short Num, short X, short Y)
    {
        // Verifica se há algum Jogador na cordenada
        for (byte i = 0; i < Lists.Player.Count; i++)
            if (Lists.Player[i].X == X && Lists.Player[i].Y == Y && Lists.Player[i].Map_Num == Num)
                return Lists.Player[i];

        return null;
    }

    private static void Fog()
    {
        // Faz a movimentação
        Calculate_Fog_X();
        Calculate_Fog_Y();
    }

    private static void Calculate_Fog_X()
    {
        Size Size = Graphics.TSize(Graphics.Tex_Fog[Lists.Map.Fog.Texture]);
        int Speed_X = Lists.Map.Fog.Speed_X;

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
        Size Size = Graphics.TSize(Graphics.Tex_Fog[Lists.Map.Fog.Texture]);
        int Speed_Y = Lists.Map.Fog.Speed_Y;

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
        if (Lists.Map.Weather.Type == 0) return;

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
        for (short i = 1; i < Lists.Weather.Length; i++)
            if (!Lists.Weather[i].Visible)
            {
                if (Game.Random.Next(0, Max_Weather_Intensity - Lists.Map.Weather.Intensity) == 0)
                {
                    if (!Stop)
                    {
                        // Cria a partícula
                        Lists.Weather[i].Visible = true;

                        // Cria a partícula de acordo com o seu tipo
                        switch ((Weathers)Lists.Map.Weather.Type)
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
                switch ((Weathers)Lists.Map.Weather.Type)
                {
                    case Weathers.Thundering:
                    case Weathers.Raining: Weather_Rain_Movement(i); break;
                    case Weathers.Snowing: Weather_Snow_Movement(i, Move); break;
                }

                // Reseta a partícula
                if (Lists.Weather[i].x > Game.Screen_Width || Lists.Weather[i].y > Game.Screen_Height)
                    Lists.Weather[i] = new Lists.Structures.Weather();
            }

        // Trovoadas
        if (Lists.Map.Weather.Type == (byte)Weathers.Thundering)
            if (Game.Random.Next(0, Max_Weather_Intensity * 10 - Lists.Map.Weather.Intensity * 2) == 0)
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
        Lists.Weather[i].Speed = Game.Random.Next(8, 13);

        if (Game.Random.Next(2) == 0)
        {
            Lists.Weather[i].x = -32;
            Lists.Weather[i].y = Game.Random.Next(-32, Game.Screen_Height);
        }
        else
        {
            Lists.Weather[i].x = Game.Random.Next(-32, Game.Screen_Width);
            Lists.Weather[i].y = -32;
        }
    }

    private static void Weather_Rain_Movement(int i)
    {
        // Movimenta a partícula
        Lists.Weather[i].x += Lists.Weather[i].Speed;
        Lists.Weather[i].y += Lists.Weather[i].Speed;
    }

    private static void Weather_Snow_Create(int i)
    {
        // Define a velocidade e a posição da partícula
        Lists.Weather[i].Speed = Game.Random.Next(1, 3);
        Lists.Weather[i].y = -32;
        Lists.Weather[i].x = Game.Random.Next(-32, Game.Screen_Width);
        Lists.Weather[i].Start = Lists.Weather[i].x;

        if (Game.Random.Next(2) == 0)
            Lists.Weather[i].Back = false;
        else
            Lists.Weather[i].Back = true;
    }

    private static void Weather_Snow_Movement(int i, bool Movimentrar = true)
    {
        int Diference = Game.Random.Next(0, Snow_Movement / 3);
        int x1 = Lists.Weather[i].Start + Snow_Movement + Diference;
        int x2 = Lists.Weather[i].Start - Snow_Movement - Diference;

        // Faz com que a partícula volte
        if (x1 <= Lists.Weather[i].x)
            Lists.Weather[i].Back = true;
        else if (x2 >= Lists.Weather[i].x)
            Lists.Weather[i].Back = false;

        // Movimenta a partícula
        Lists.Weather[i].y += Lists.Weather[i].Speed;

        if (Movimentrar)
            if (Lists.Weather[i].Back)
                Lists.Weather[i].x -= 1;
            else
                Lists.Weather[i].x += 1;
    }

    public static void Weather_Update()
    {
        // Para todos os sons
        Audio.Sound.Stop_All();

        // Redimensiona a lista
        switch ((Weathers)Lists.Map.Weather.Type)
        {
            case Weathers.Thundering:
            case Weathers.Raining:
                // Reproduz o som chuva
                Audio.Sound.Play(Audio.Sounds.Rain, true);

                // Redimensiona a estrutura
                Lists.Weather = new Lists.Structures.Weather[Max_Rain + 1];
                break;
            case Weathers.Snowing: Lists.Weather = new Lists.Structures.Weather[Max_Snow + 1]; break;
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
            for (byte x = 0; x <= Lists.Map.Width; x++)
                for (byte y = 0; y <= Lists.Map.Height; y++)
                    for (byte c = 0; c < (byte)Layers.Amount; c++)
                        for (byte q = 0; q <= Lists.Map.Tile[x, y].Data.GetUpperBound(1); q++)
                            if (Lists.Map.Tile[x, y].Data[c, q].Automatic)
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
            Lists.Structures.Map_Tile_Data Data = Lists.Map.Tile[x, y].Data[Layer_Type, Layer_Num];
            Lists.Map.Tile[x, y].Data[Layer_Type, Layer_Num].Mini[Part].X = Data.X * Game.Grid + Position.X;
            Lists.Map.Tile[x, y].Data[Layer_Type, Layer_Num].Mini[Part].Y = Data.Y * Game.Grid + Position.Y;
        }

        private static bool Check(int X1, int Y1, int X2, int Y2, byte Layer_Num, byte Layer_Type)
        {
            Lists.Structures.Map_Tile_Data Data1, Data2;

            // Somente se necessário
            if (X2 < 0 || X2 > Lists.Map.Width || Y2 < 0 || Y2 > Lists.Map.Height) return true;

            // Dados
            Data1 = Lists.Map.Tile[X1, Y1].Data[Layer_Type, Layer_Num];
            Data2 = Lists.Map.Tile[X2, Y2].Data[Layer_Type, Layer_Num];

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

partial class Graphics
{
    private static void Map_Tiles(byte c)
    {
        // Previne erros
        if (Lists.Map.Name == null) return;

        // Dados
        System.Drawing.Color TempColor = System.Drawing.Color.FromArgb(Lists.Map.Color);
        SFML.Graphics.Color Color = CColor(TempColor.R, TempColor.G, TempColor.B);

        // Desenha todas as camadas dos azulejos
        for (short x = (short)Game.Sight.X; x <= Game.Sight.Width; x++)
            for (short y = (short)Game.Sight.Y; y <= Game.Sight.Height; y++)
                if (!Map.OutOfLimit(x, y))
                    for (byte q = 0; q <= Lists.Map.Tile[x, y].Data.GetUpperBound(1); q++)
                        if (Lists.Map.Tile[x, y].Data[c, q].Tile > 0)
                        {
                            int x2 = Lists.Map.Tile[x, y].Data[c, q].X * Game.Grid;
                            int y2 = Lists.Map.Tile[x, y].Data[c, q].Y * Game.Grid;

                            // Desenha o azulejo
                            if (!Lists.Map.Tile[x, y].Data[c, q].Automatic)
                                Render(Tex_Tile[Lists.Map.Tile[x, y].Data[c, q].Tile], Game.ConvertX(x * Game.Grid), Game.ConvertY(y * Game.Grid), x2, y2, Game.Grid, Game.Grid, Color);
                            else
                                Map_Autotile(new Point(Game.ConvertX(x * Game.Grid), Game.ConvertY(y * Game.Grid)), Lists.Map.Tile[x, y].Data[c, q], Color);
                        }
    }

    private static void Map_Autotile(Point Position, Lists.Structures.Map_Tile_Data Dados, SFML.Graphics.Color Cor)
    {
        // Desenha os 4 mini azulejos
        for (byte i = 0; i <= 3; i++)
        {
            Point Destiny = Position, Source = Dados.Mini[i];

            // Partes do azulejo
            switch (i)
            {
                case 1: Destiny.X += 16; break;
                case 2: Destiny.Y += 16; break;
                case 3: Destiny.X += 16; Destiny.Y += 16; break;
            }

            // Renderiza o mini azulejo
            Render(Tex_Tile[Dados.Tile], new Rectangle(Source.X, Source.Y, 16, 16), new Rectangle(Destiny, new Size(16, 16)), Cor);
        }
    }

    private static void Map_Panorama()
    {
        // Desenha o panorama
        if (Lists.Map.Panorama > 0)
            Render(Tex_Panorama[Lists.Map.Panorama], new Point(0));
    }

    private static void Map_Fog()
    {
        Lists.Structures.Map_Fog Data = Lists.Map.Fog;
        Size Texture_Size = TSize(Tex_Fog[Data.Texture]);

        // Previne erros
        if (Data.Texture <= 0) return;

        // Desenha a fumaça
        for (int x = -1; x <= Lists.Map.Width * Game.Grid / Texture_Size.Width + 1; x++)
            for (int y = -1; y <= Lists.Map.Height * Game.Grid / Texture_Size.Height + 1; y++)
                Render(Tex_Fog[Data.Texture], new Point(x * Texture_Size.Width + Map.Fog_X, y * Texture_Size.Height + Map.Fog_Y), new SFML.Graphics.Color(255, 255, 255, Data.Alpha));
    }

    private static void Map_Weather()
    {
        byte x = 0;

        // Somente se necessário
        if (Lists.Map.Weather.Type == 0) return;

        // Textura
        switch ((Map.Weathers)Lists.Map.Weather.Type)
        {
            case Map.Weathers.Snowing: x = 32; break;
        }

        // Desenha as partículas
        for (int i = 1; i < Lists.Weather.Length; i++)
            if (Lists.Weather[i].Visible)
                Render(Tex_Weather, new Rectangle(x, 0, 32, 32), new Rectangle(Lists.Weather[i].x, Lists.Weather[i].y, 32, 32), CColor(255, 255, 255, 150));

        // Trovoadas
        Render(Tex_Blanc, 0, 0, 0, 0, Game.Screen_Width, Game.Screen_Height, new SFML.Graphics.Color(255, 255, 255, Map.Lightning));
    }

    private static void Map_Name()
    {
        SFML.Graphics.Color Color;

        // Somente se necessário
        if (string.IsNullOrEmpty(Lists.Map.Name)) return;

        // A cor do texto vária de acordo com a moral do mapa
        switch (Lists.Map.Moral)
        {
            case (byte)Map.Morals.Danger: Color = SFML.Graphics.Color.Red; break;
            default: Color = SFML.Graphics.Color.White; break;
        }

        // Desenha o nome do mapa
        DrawText(Lists.Map.Name, 426, 48, Color);
    }

    private static void Map_Items()
    {
        // Desenha todos os itens que estão no chão
        for (byte i = 1; i < Lists.Temp_Map.Item.Length; i++)
        {
            Lists.Structures.Map_Items Data = Lists.Temp_Map.Item[i];

            // Somente se necessário
            if (Data.Index == 0) continue;

            // Desenha o item
            Point Position = new Point(Game.ConvertX(Data.X * Game.Grid), Game.ConvertY(Data.Y * Game.Grid));
            Render(Tex_Item[Lists.Item[Data.Index].Texture], Position);
        }
    }

    private static void Map_Blood()
    {
        // Desenha todos os sangues
        for (byte i = 0; i < Lists.Temp_Map.Blood.Count; i++)
        {
            Lists.Structures.Map_Blood Data = Lists.Temp_Map.Blood[i];
            Render(Tex_Blood, Game.ConvertX(Data.X * Game.Grid), Game.ConvertY(Data.Y * Game.Grid), Data.Texture_Num * 32, 0, 32, 32, CColor(255, 255, 255, Data.Opacity));
        }
    }
}