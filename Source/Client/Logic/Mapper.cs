using CryBits.Client.Entities;
using CryBits.Client.Logic;
using System;
using System.Drawing;
using static CryBits.Client.Logic.Game;
using static CryBits.Client.Logic.Utils;

class Mapper
{
    // Mapa atual
    public static TempMap Current;

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

    // Verifica se as coordenas estão no limite do mapa
    public static bool OutOfLimit(int x, int y) => x >= Map_Width || y >= Map_Height || x < 0 || y < 0;

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
        for (short i = 1; i < TempMap.Weather.Length; i++)
            if (!TempMap.Weather[i].Visible)
            {
                if (MyRandom.Next(0, Max_Weather_Intensity - Current.Data.Weather.Intensity) == 0)
                {
                    if (!Stop)
                    {
                        // Cria a partícula
                        TempMap.Weather[i].Visible = true;

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
                if (TempMap.Weather[i].x > Screen_Width || TempMap.Weather[i].y > Screen_Height)
                    TempMap.Weather[i] = new TMap_Weather();
            }

        // Trovoadas
        if (Current.Data.Weather.Type == (byte)Weathers.Thundering)
            if (MyRandom.Next(0, Max_Weather_Intensity * 10 - Current.Data.Weather.Intensity * 2) == 0)
            {
                // Som do trovão
                int Thunder = MyRandom.Next(Thunder_First, Thunder_Last);
                Audio.Sound.Play((Audio.Sounds)Thunder);

                // Relâmpago
                if (Thunder < 6) Lightning = 190;
            }
    }

    private static void Weather_Rain_Create(int i)
    {
        // Define a velocidade e a posição da partícula
        TempMap.Weather[i].Speed = MyRandom.Next(8, 13);

        if (MyRandom.Next(2) == 0)
        {
            TempMap.Weather[i].x = -32;
            TempMap.Weather[i].y = MyRandom.Next(-32, Screen_Height);
        }
        else
        {
            TempMap.Weather[i].x = MyRandom.Next(-32, Screen_Width);
            TempMap.Weather[i].y = -32;
        }
    }

    private static void Weather_Rain_Movement(int i)
    {
        // Movimenta a partícula
        TempMap.Weather[i].x += TempMap.Weather[i].Speed;
        TempMap.Weather[i].y += TempMap.Weather[i].Speed;
    }

    private static void Weather_Snow_Create(int i)
    {
        // Define a velocidade e a posição da partícula
        TempMap.Weather[i].Speed = MyRandom.Next(1, 3);
        TempMap.Weather[i].y = -32;
        TempMap.Weather[i].x = MyRandom.Next(-32, Screen_Width);
        TempMap.Weather[i].Start = TempMap.Weather[i].x;

        if (MyRandom.Next(2) == 0)
            TempMap.Weather[i].Back = false;
        else
            TempMap.Weather[i].Back = true;
    }

    private static void Weather_Snow_Movement(int i, bool Movimentrar = true)
    {
        int Diference = MyRandom.Next(0, Snow_Movement / 3);
        int x1 = TempMap.Weather[i].Start + Snow_Movement + Diference;
        int x2 = TempMap.Weather[i].Start - Snow_Movement - Diference;

        // Faz com que a partícula volte
        if (x1 <= TempMap.Weather[i].x)
            TempMap.Weather[i].Back = true;
        else if (x2 >= TempMap.Weather[i].x)
            TempMap.Weather[i].Back = false;

        // Movimenta a partícula
        TempMap.Weather[i].y += TempMap.Weather[i].Speed;

        if (Movimentrar)
            if (TempMap.Weather[i].Back)
                TempMap.Weather[i].x -= 1;
            else
                TempMap.Weather[i].x += 1;
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
                TempMap.Weather = new TMap_Weather[Max_Rain + 1];
                break;
            case Weathers.Snowing: TempMap.Weather = new TMap_Weather[Max_Snow + 1]; break;
        }
    }
}