using System;
using System.Drawing;
using CryBits.Client.Entities;
using CryBits.Client.Media.Audio;
using static CryBits.Client.Logic.Game;
using static CryBits.Utils;
using Graphics = CryBits.Client.Media.Graphics;

namespace CryBits.Client.Logic
{
    internal static class Mapper
    {
        // Mapa atual
        public static TempMap Current;

        // Fumaças
        public static int FogX;
        public static int FogY;
        private static int _fogXTimer;
        private static int _fogYTimer;

        // Clima
        public const byte MaxRain = 100;
        public const short MaxSnow = 635;
        public const byte MaxWeatherIntensity = 10;
        public const byte SnowMovement = 10;
        public static byte Lightning;
        private static int _snowTimer;
        private static int _lightningTimer;

        // Sangue
        private static int _bloodTimer;

        public static void Logic()
        {
            // Toda a lógica do mapa
            Fog();
            Weather();

            // Retira os sangues do chão depois de um determinado tempo
            if (_bloodTimer < Environment.TickCount)
                for (byte i = 0; i < Current.Blood.Count; i++)
                {
                    Current.Blood[i].Opacity -= 1;
                    if (Current.Blood[i].Opacity == 0) Current.Blood.RemoveAt(i);
                    _bloodTimer = Environment.TickCount + 100;
                }
        }

        // Verifica se as coordenas estão no limite do mapa
        public static bool OutOfLimit(int x, int y) => x >= MapWidth || y >= MapHeight || x < 0 || y < 0;

        private static void Fog()
        {
            // Faz a movimentação
            Calculate_Fog_X();
            Calculate_Fog_Y();
        }

        private static void Calculate_Fog_X()
        {
            Size size = Graphics.Size(Graphics.TexFog[Current.Data.Fog.Texture]);
            int speedX = Current.Data.Fog.SpeedX;

            // Apenas se necessário
            if (_fogXTimer >= Environment.TickCount) return;
            if (speedX == 0) return;

            // Movimento para trás
            if (speedX < 0)
            {
                FogX -= 1;
                if (FogX < -size.Width) FogX = 0;
            }
            // Movimento para frente
            else
            {
                FogX += 1;
                if (FogX > size.Width) FogX = 0;
            }

            // Contagem
            if (speedX < 0) speedX *= -1;
            _fogXTimer = Environment.TickCount + 50 - speedX;
        }

        private static void Calculate_Fog_Y()
        {
            Size size = Graphics.Size(Graphics.TexFog[Current.Data.Fog.Texture]);
            int speedY = Current.Data.Fog.SpeedY;

            // Apenas se necessário
            if (_fogYTimer >= Environment.TickCount) return;
            if (speedY == 0) return;

            // Movimento para trás
            if (speedY < 0)
            {
                FogY -= 1;
                if (FogY < -size.Height) FogY = 0;
            }
            // Movimento para frente
            else
            {
                FogY += 1;
                if (FogY > size.Height) FogY = 0;
            }

            // Contagem
            if (speedY < 0) speedY *= -1;
            _fogYTimer = Environment.TickCount + 50 - speedY;
        }

        private static void Weather()
        {
            bool stop = false, move;
            byte thunderFirst = (byte)Sounds.Thunder1;
            byte thunderLast = (byte)Sounds.Thunder4;

            // Somente se necessário
            if (Current.Data.Weather.Type == 0) return;

            // Contagem da neve
            if (_snowTimer < Environment.TickCount)
            {
                move = true;
                _snowTimer = Environment.TickCount + 35;
            }
            else
                move = false;

            // Contagem dos relâmpagos
            if (Lightning > 0)
                if (_lightningTimer < Environment.TickCount)
                {
                    Lightning -= 10;
                    _lightningTimer = Environment.TickCount + 25;
                }

            // Adiciona uma nova partícula
            for (short i = 1; i < TempMap.Weather.Length; i++)
                if (!TempMap.Weather[i].Visible)
                {
                    if (MyRandom.Next(0, MaxWeatherIntensity - Current.Data.Weather.Intensity) == 0)
                    {
                        if (!stop)
                        {
                            // Cria a partícula
                            TempMap.Weather[i].Visible = true;

                            // Cria a partícula de acordo com o seu tipo
                            switch (Current.Data.Weather.Type)
                            {
                                case Weathers.Thundering:
                                case Weathers.Raining: Weather_Rain_Create(i); break;
                                case Weathers.Snowing: Weather_Snow_Create(i); break;
                            }
                        }
                    }

                    stop = true;
                }
                else
                {
                    // Movimenta a partícula de acordo com o seu tipo
                    switch (Current.Data.Weather.Type)
                    {
                        case Weathers.Thundering:
                        case Weathers.Raining: Weather_Rain_Movement(i); break;
                        case Weathers.Snowing: Weather_Snow_Movement(i, move); break;
                    }

                    // Reseta a partícula
                    if (TempMap.Weather[i].X > ScreenWidth || TempMap.Weather[i].Y > ScreenHeight)
                        TempMap.Weather[i] = new MapWeatherParticle();
                }

            // Trovoadas
            if (Current.Data.Weather.Type == Weathers.Thundering)
                if (MyRandom.Next(0, MaxWeatherIntensity * 10 - Current.Data.Weather.Intensity * 2) == 0)
                {
                    // Som do trovão
                    int thunder = MyRandom.Next(thunderFirst, thunderLast);
                    Sound.Play((Sounds)thunder);

                    // Relâmpago
                    if (thunder < 6) Lightning = 190;
                }
        }

        private static void Weather_Rain_Create(int i)
        {
            // Define a velocidade e a posição da partícula
            TempMap.Weather[i].Speed = MyRandom.Next(8, 13);

            if (MyRandom.Next(2) == 0)
            {
                TempMap.Weather[i].X = -32;
                TempMap.Weather[i].Y = MyRandom.Next(-32, ScreenHeight);
            }
            else
            {
                TempMap.Weather[i].X = MyRandom.Next(-32, ScreenWidth);
                TempMap.Weather[i].Y = -32;
            }
        }

        private static void Weather_Rain_Movement(int i)
        {
            // Movimenta a partícula
            TempMap.Weather[i].X += TempMap.Weather[i].Speed;
            TempMap.Weather[i].Y += TempMap.Weather[i].Speed;
        }

        private static void Weather_Snow_Create(int i)
        {
            // Define a velocidade e a posição da partícula
            TempMap.Weather[i].Speed = MyRandom.Next(1, 3);
            TempMap.Weather[i].Y = -32;
            TempMap.Weather[i].X = MyRandom.Next(-32, ScreenWidth);
            TempMap.Weather[i].Start = TempMap.Weather[i].X;
            TempMap.Weather[i].Back = MyRandom.Next(2) != 0;
        }

        private static void Weather_Snow_Movement(int i, bool move = true)
        {
            int difference = MyRandom.Next(0, SnowMovement / 3);
            int x1 = TempMap.Weather[i].Start + SnowMovement + difference;
            int x2 = TempMap.Weather[i].Start - SnowMovement - difference;

            // Faz com que a partícula volte
            if (x1 <= TempMap.Weather[i].X)
                TempMap.Weather[i].Back = true;
            else if (x2 >= TempMap.Weather[i].X)
                TempMap.Weather[i].Back = false;

            // Movimenta a partícula
            TempMap.Weather[i].Y += TempMap.Weather[i].Speed;

            if (move)
                if (TempMap.Weather[i].Back)
                    TempMap.Weather[i].X -= 1;
                else
                    TempMap.Weather[i].X += 1;
        }

        public static void Weather_Update()
        {
            // Para todos os sons
            Sound.Stop_All();
            if (Current == null) return;
            // Redimensiona a lista
            switch (Current.Data.Weather.Type)
            {
                case Weathers.Thundering:
                case Weathers.Raining:
                    // Reproduz o som chuva
                    Sound.Play(Sounds.Rain, true);

                    // Redimensiona a estrutura
                    TempMap.Weather = new MapWeatherParticle[MaxRain + 1];
                    break;
                case Weathers.Snowing: TempMap.Weather = new MapWeatherParticle[MaxSnow + 1]; break;
            }
        }
    }
}