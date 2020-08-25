using Entities;
using System;
using System.Drawing;
using System.Windows.Forms;
using static Utils;

class Loop
{
    // Contadores
    private static int FogX_Timer = 0;
    private static int FogY_Timer = 0;
    private static int Snow_Timer = 0;
    private static int Thundering_Timer = 0;

    public static void Init()
    {
        int Count;
        int Timer_1000 = 0;
        short FPS = 0;

        while (Program.Working)
        {
            Count = Environment.TickCount;

            // Manuseia os dados recebidos
            Network.Socket.HandleData();

            // Eventos
            Editor_Maps_Fog();
            Editor_Maps_Weather();
            Editor_Maps_Music();

            // Desenha os gráficos
            Graphics.Present();

            // Faz com que a aplicação se mantenha estável
            Application.DoEvents();
            while (Environment.TickCount < Count + 10) System.Threading.Thread.Sleep(1);

            // FPS
            if (Timer_1000 < Environment.TickCount)
            {
                // Cálcula o FPS
                Program.FPS = FPS;
                FPS = 0;

                // Reinicia a contagem
                Timer_1000 = Environment.TickCount + 1000;
            }
            else
                FPS += 1;
        }

        // Fecha a aplicação
        Program.Close();
    }

    private static void Editor_Maps_Fog()
    {
        // Faz a movimentação
        if (Editor_Maps.Form != null)
        {
            Editor_Maps_Fog_X();
            Editor_Maps_Fog_Y();
        }
    }

    private static void Editor_Maps_Fog_X()
    {
        Size Texture_Size = Graphics.TSize(Graphics.Tex_Fog[Editor_Maps.Form.Selected.Fog.Texture]);
        int Speed = Editor_Maps.Form.Selected.Fog.Speed_X;

        // Apenas se necessário
        if (FogX_Timer >= Environment.TickCount) return;
        if (Speed == 0) return;

        // Movimento para trás
        if (Speed < 0)
        {
            Map.Fog_X -= 1;
            if (Map.Fog_X < -Texture_Size.Width) Map.Fog_X = 0;
        }
        // Movimento para frente
        else
        {
            Map.Fog_X += 1;
            if (Map.Fog_X > Texture_Size.Width) Map.Fog_X = 0;
        }

        // Contagem
        if (Speed < 0) Speed *= -1;
        FogX_Timer = Environment.TickCount + 50 - Speed;
    }

    private static void Editor_Maps_Fog_Y()
    {
        Size Texture_Size = Graphics.TSize(Graphics.Tex_Fog[Editor_Maps.Form.Selected.Fog.Texture]);
        int Speed = Editor_Maps.Form.Selected.Fog.Speed_Y;

        // Apenas se necessário
        if (FogY_Timer >= Environment.TickCount) return;
        if (Speed == 0) return;

        // Movimento para trás
        if (Speed < 0)
        {
            Map.Fog_Y -= 1;
            if (Map.Fog_Y < -Texture_Size.Height) Map.Fog_Y = 0;
        }
        // Movimento para frente
        else
        {
            Map.Fog_Y += 1;
            if (Map.Fog_Y > Texture_Size.Height) Map.Fog_Y = 0;
        }

        // Contagem
        if (Speed < 0) Speed *= -1;
        FogY_Timer = Environment.TickCount + 50 - Speed;
    }

    private static void Editor_Maps_Weather()
    {
        bool Stop = false, Move;

        // Somente se necessário
        if (Editor_Maps.Form == null || Editor_Maps.Form.Selected.Weather.Type == 0 || !Editor_Maps.Form.butVisualization.Checked)
        {
            if (Audio.Sound.List != null)
                if (Audio.Sound.List[(byte)Audio.Sounds.Rain].Status == SFML.Audio.SoundStatus.Playing) Audio.Sound.Stop_All();
            return;
        }

        // Clima do mapa
        Map_Weather Weather = Editor_Maps.Form.Selected.Weather;

        // Reproduz o som chuva
        if (Weather.Type == Weathers.Raining || Weather.Type == Weathers.Thundering)
        {
            if (Audio.Sound.List[(byte)Audio.Sounds.Rain].Status != SFML.Audio.SoundStatus.Playing)
                Audio.Sound.Play(Audio.Sounds.Rain);
        }
        else
          if (Audio.Sound.List[(byte)Audio.Sounds.Rain].Status == SFML.Audio.SoundStatus.Playing) Audio.Sound.Stop_All();

        // Contagem da neve
        if (Snow_Timer < Environment.TickCount)
        {
            Move = true;
            Snow_Timer = Environment.TickCount + 35;
        }
        else
            Move = false;

        // Contagem dos relâmpagos
        if (Map.Lightning > 0)
            if (Thundering_Timer < Environment.TickCount)
            {
                Map.Lightning -= 10;
                Thundering_Timer = Environment.TickCount + 25;
            }

        // Adiciona uma nova partícula
        for (int i = 1; i <= Lists.Weather.GetUpperBound(0); i++)
            if (!Lists.Weather[i].Visible)
            {
                if (MyRandom.Next(0, Map.Max_Weather_Intensity - Weather.Intensity) == 0)
                {
                    if (!Stop)
                    {
                        // Cria a partícula
                        Lists.Weather[i].Visible = true;

                        // Cria a partícula de acordo com o seu tipo
                        switch (Weather.Type)
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
                switch (Weather.Type)
                {
                    case Weathers.Thundering:
                    case Weathers.Raining: Weather_Rain_Movement(i); break;
                    case Weathers.Snowing: Weather_Snow_Movement(i, Move); break;
                }

                // Reseta a partícula
                if (Lists.Weather[i].x > Map.Width * Grid || Lists.Weather[i].y > Map.Height * Grid)
                    Lists.Weather[i] = new Lists.Structures.Weather();
            }

        // Trovoadas
        if (Weather.Type == Weathers.Thundering)
            if (MyRandom.Next(0, Map.Max_Weather_Intensity * 10 - Weather.Intensity * 2) == 0)
            {
                // Som do trovão
                int Thunder = MyRandom.Next((byte)Audio.Sounds.Thunder_1, (byte)Audio.Sounds.Thunder_4);
                Audio.Sound.Play((Audio.Sounds)Thunder);

                // Relâmpago
                if (Thunder < 6) Map.Lightning = 190;
            }
    }

    private static void Weather_Rain_Create(int i)
    {
        // Define a velocidade e a posição da partícula
        Lists.Weather[i].Speed = MyRandom.Next(8, 13);

        if (MyRandom.Next(2) == 0)
        {
            Lists.Weather[i].x = -32;
            Lists.Weather[i].y = MyRandom.Next(-32, Editor_Maps.Form.picMap.Height);
        }
        else
        {
            Lists.Weather[i].x = MyRandom.Next(-32, Editor_Maps.Form.picMap.Width);
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
        Lists.Weather[i].Speed = MyRandom.Next(1, 3);
        Lists.Weather[i].y = -32;
        Lists.Weather[i].x = MyRandom.Next(-32, Editor_Maps.Form.picMap.Width);
        Lists.Weather[i].Start = Lists.Weather[i].x;

        if (MyRandom.Next(2) == 0)
            Lists.Weather[i].Back = false;
        else
            Lists.Weather[i].Back = true;
    }

    private static void Weather_Snow_Movement(int i, bool Move = true)
    {
        int Difference = MyRandom.Next(0, Map.Snow_Movement / 3);
        int x1 = Lists.Weather[i].Start + Map.Snow_Movement + Difference;
        int x2 = Lists.Weather[i].Start - Map.Snow_Movement - Difference;

        // Faz com que a partícula volte
        if (x1 <= Lists.Weather[i].x)
            Lists.Weather[i].Back = true;
        else if (x2 >= Lists.Weather[i].x)
            Lists.Weather[i].Back = false;

        // Movimenta a partícula
        Lists.Weather[i].y += Lists.Weather[i].Speed;

        if (Move)
            if (Lists.Weather[i].Back)
                Lists.Weather[i].x -= 1;
            else
                Lists.Weather[i].x += 1;
    }

    private static void Editor_Maps_Music()
    {
        // Apenas se necessário
        if (Editor_Maps.Form == null) goto stop;
        if (!Editor_Maps.Form.butAudio.Checked) goto stop;
        if (!Editor_Maps.Form.butVisualization.Checked) goto stop;
        if (Editor_Maps.Form.Selected.Music == 0) goto stop;

        // Inicia a música
        if (Audio.Music.Device == null || Audio.Music.Current != Editor_Maps.Form.Selected.Music)
            Audio.Music.Play(Editor_Maps.Form.Selected.Music);
        return;
    stop:
        // Para a música
        if (Audio.Music.Device != null) Audio.Music.Stop();
    }
}
