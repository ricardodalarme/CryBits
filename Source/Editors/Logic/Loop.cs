using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using CryBits.Editors.Entities;
using CryBits.Editors.Forms;
using CryBits.Editors.Library;
using CryBits.Editors.Media.Audio;
using CryBits.Editors.Network;
using CryBits.Entities;
using SFML.Audio;
using CryBits;
using static CryBits.Editors.Logic.Utils;
using Graphics = CryBits.Editors.Media.Graphics;
using Music = CryBits.Editors.Media.Audio.Music;
using Sound = CryBits.Editors.Media.Audio.Sound;

namespace CryBits.Editors.Logic
{
    internal class Loop
    {
        // Contadores
        private static int _fogXTimer;
        private static int _fogYTimer;
        private static int _snowTimer;
        private static int _thunderingTimer;

        public static void Init()
        {
            int count;
            int timer1000 = 0;
            short fps = 0;

            while (Program.Working)
            {
                count = Environment.TickCount;

                // Manuseia os dados recebidos
                Socket.HandleData();

                // Eventos
                Editor_Maps_Fog();
                Editor_Maps_Weather();
                Editor_Maps_Music();

                // Desenha os gráficos
                Graphics.Present();

                // Faz com que a aplicação se mantenha estável
                Application.DoEvents();
                while (Environment.TickCount < count + 10) Thread.Sleep(1);

                // FPS
                if (timer1000 < Environment.TickCount)
                {
                    // Cálcula o FPS
                    Program.FPS = fps;
                    fps = 0;

                    // Reinicia a contagem
                    timer1000 = Environment.TickCount + 1000;
                }
                else
                    fps += 1;
            }

            // Fecha a aplicação
            Program.Close();
        }

        private static void Editor_Maps_Fog()
        {
            // Faz a movimentação
            if (EditorMaps.Form != null && EditorMaps.Form.Visible)
            {
                Editor_Maps_Fog_X();
                Editor_Maps_Fog_Y();
            }
        }

        private static void Editor_Maps_Fog_X()
        {
            Size textureSize = Graphics.Size(Graphics.Tex_Fog[EditorMaps.Form.Selected.Fog.Texture]);
            int speed = EditorMaps.Form.Selected.Fog.Speed_X;

            // Apenas se necessário
            if (_fogXTimer >= Environment.TickCount) return;
            if (speed == 0) return;

            // Movimento para trás
            if (speed < 0)
            {
                TempMap.Fog_X -= 1;
                if (TempMap.Fog_X < -textureSize.Width) TempMap.Fog_X = 0;
            }
            // Movimento para frente
            else
            {
                TempMap.Fog_X += 1;
                if (TempMap.Fog_X > textureSize.Width) TempMap.Fog_X = 0;
            }

            // Contagem
            if (speed < 0) speed *= -1;
            _fogXTimer = Environment.TickCount + 50 - speed;
        }

        private static void Editor_Maps_Fog_Y()
        {
            Size textureSize = Graphics.Size(Graphics.Tex_Fog[EditorMaps.Form.Selected.Fog.Texture]);
            int speed = EditorMaps.Form.Selected.Fog.Speed_Y;

            // Apenas se necessário
            if (_fogYTimer >= Environment.TickCount) return;
            if (speed == 0) return;

            // Movimento para trás
            if (speed < 0)
            {
                TempMap.Fog_Y -= 1;
                if (TempMap.Fog_Y < -textureSize.Height) TempMap.Fog_Y = 0;
            }
            // Movimento para frente
            else
            {
                TempMap.Fog_Y += 1;
                if (TempMap.Fog_Y > textureSize.Height) TempMap.Fog_Y = 0;
            }

            // Contagem
            if (speed < 0) speed *= -1;
            _fogYTimer = Environment.TickCount + 50 - speed;
        }

        private static void Editor_Maps_Weather()
        {
            bool stop = false, move;

            // Somente se necessário
            if (EditorMaps.Form == null || !EditorMaps.Form.Visible || EditorMaps.Form.Selected.Weather.Type == 0 || !EditorMaps.Form.butVisualization.Checked)
            {
                if (Sound.List != null)
                    if (Sound.List[(byte)Sounds.Rain].Status == SoundStatus.Playing) Sound.Stop_All();
                return;
            }

            // Clima do mapa
            MapWeather weather = EditorMaps.Form.Selected.Weather;

            // Reproduz o som chuva
            if (weather.Type == Weathers.Raining || weather.Type == Weathers.Thundering)
            {
                if (Sound.List[(byte)Sounds.Rain].Status != SoundStatus.Playing)
                    Sound.Play(Sounds.Rain);
            }
            else
              if (Sound.List[(byte)Sounds.Rain].Status == SoundStatus.Playing) Sound.Stop_All();

            // Contagem da neve
            if (_snowTimer < Environment.TickCount)
            {
                move = true;
                _snowTimer = Environment.TickCount + 35;
            }
            else
                move = false;

            // Contagem dos relâmpagos
            if (TempMap.Lightning > 0)
                if (_thunderingTimer < Environment.TickCount)
                {
                    TempMap.Lightning -= 10;
                    _thunderingTimer = Environment.TickCount + 25;
                }

            // Adiciona uma nova partícula
            for (int i = 1; i <= Lists.Weather.GetUpperBound(0); i++)
                if (!Lists.Weather[i].Visible)
                {
                    if (MyRandom.Next(0, Map.MaxWeatherIntensity - weather.Intensity) == 0)
                    {
                        if (!stop)
                        {
                            // Cria a partícula
                            Lists.Weather[i].Visible = true;

                            // Cria a partícula de acordo com o seu tipo
                            switch (weather.Type)
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
                    switch (weather.Type)
                    {
                        case Weathers.Thundering:
                        case Weathers.Raining: Weather_Rain_Movement(i); break;
                        case Weathers.Snowing: Weather_Snow_Movement(i, move); break;
                    }

                    // Reseta a partícula
                    if (Lists.Weather[i].X > Map.Width * Grid || Lists.Weather[i].Y > Map.Height * Grid)
                        Lists.Weather[i] = new MapWeatherParticle();
                }

            // Trovoadas
            if (weather.Type == Weathers.Thundering)
                if (MyRandom.Next(0, Map.MaxWeatherIntensity * 10 - weather.Intensity * 2) == 0)
                {
                    // Som do trovão
                    int thunder = MyRandom.Next((byte)Sounds.Thunder_1, (byte)Sounds.Thunder_4);
                    Sound.Play((Sounds)thunder);

                    // Relâmpago
                    if (thunder < 6) TempMap.Lightning = 190;
                }
        }

        private static void Weather_Rain_Create(int i)
        {
            // Define a velocidade e a posição da partícula
            Lists.Weather[i].Speed = MyRandom.Next(8, 13);

            if (MyRandom.Next(2) == 0)
            {
                Lists.Weather[i].X = -32;
                Lists.Weather[i].Y = MyRandom.Next(-32, EditorMaps.Form.picMap.Height);
            }
            else
            {
                Lists.Weather[i].X = MyRandom.Next(-32, EditorMaps.Form.picMap.Width);
                Lists.Weather[i].Y = -32;
            }
        }

        private static void Weather_Rain_Movement(int i)
        {
            // Movimenta a partícula
            Lists.Weather[i].X += Lists.Weather[i].Speed;
            Lists.Weather[i].Y += Lists.Weather[i].Speed;
        }

        private static void Weather_Snow_Create(int i)
        {
            // Define a velocidade e a posição da partícula
            Lists.Weather[i].Speed = MyRandom.Next(1, 3);
            Lists.Weather[i].Y = -32;
            Lists.Weather[i].X = MyRandom.Next(-32, EditorMaps.Form.picMap.Width);
            Lists.Weather[i].Start = Lists.Weather[i].X;

            if (MyRandom.Next(2) == 0)
                Lists.Weather[i].Back = false;
            else
                Lists.Weather[i].Back = true;
        }

        private static void Weather_Snow_Movement(int i, bool move = true)
        {
            int difference = MyRandom.Next(0, Map.SnowMovement / 3);
            int x1 = Lists.Weather[i].Start + Map.SnowMovement + difference;
            int x2 = Lists.Weather[i].Start - Map.SnowMovement - difference;

            // Faz com que a partícula volte
            if (x1 <= Lists.Weather[i].X)
                Lists.Weather[i].Back = true;
            else if (x2 >= Lists.Weather[i].X)
                Lists.Weather[i].Back = false;

            // Movimenta a partícula
            Lists.Weather[i].Y += Lists.Weather[i].Speed;

            if (move)
                if (Lists.Weather[i].Back)
                    Lists.Weather[i].X -= 1;
                else
                    Lists.Weather[i].X += 1;
        }

        private static void Editor_Maps_Music()
        {
            // Apenas se necessário
            if (EditorMaps.Form == null || !EditorMaps.Form.Visible) goto stop;
            if (!EditorMaps.Form.butAudio.Checked) goto stop;
            if (!EditorMaps.Form.butVisualization.Checked) goto stop;
            if (EditorMaps.Form.Selected.Music == 0) goto stop;

            // Inicia a música
            if (Music.Device == null || Music.Current != (Musics)EditorMaps.Form.Selected.Music)
                Music.Play((Musics)EditorMaps.Form.Selected.Music);
            return;
        stop:
            // Para a música
            if (Music.Device != null) Music.Stop();
        }
    }
}