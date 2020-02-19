using System;
using System.Drawing;
using System.Windows.Forms;

class Loop
{
    // Contadores
    private static int FogX_Timer = 0;
    private static int FogY_Timer = 0;
    private static int Snow_Timer = 0;
    private static int Thundering_Timer = 0;
    private static byte Anim = 0;

    public static void Init()
    {
        int Count;
        int Timer_1000 = 0;
        short FPS = 0;

        while (Program.Working)
        {
            Count = Environment.TickCount;

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
            while (Environment.TickCount < Count + 10) System.Threading.Thread.Sleep(1);

            // FPS
            if (Timer_1000 < Environment.TickCount)
            {
                // Cálcula o FPS
                Globals.FPS = FPS;
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
        if (Editor_Maps.Objects.Visible)
        {
            Editor_Maps_Fog_X();
            Editor_Maps_Fog_Y();
        }
    }

    private static void Editor_Maps_Fog_X()
    {
        Size Texture_Size = Graphics.TSize(Graphics.Tex_Fog[Lists.Map[Editor_Maps.Selected].Fog.Texture]);
        int Speed = Lists.Map[Editor_Maps.Selected].Fog.Speed_X;

        // Apenas se necessário
        if (FogX_Timer >= Environment.TickCount) return;
        if (Speed == 0) return;

        // Movimento para trás
        if (Speed < 0)
        {
            Globals.Fog_X -= 1;
            if (Globals.Fog_X < -Texture_Size.Width) Globals.Fog_X = 0;
        }
        // Movimento para frente
        else
        {
            Globals.Fog_X += 1;
            if (Globals.Fog_X > Texture_Size.Width) Globals.Fog_X = 0;
        }

        // Contagem
        if (Speed < 0) Speed *= -1;
        FogX_Timer = Environment.TickCount + 50 - Speed;
    }

    private static void Editor_Maps_Fog_Y()
    {
        Size Texture_Size = Graphics.TSize(Graphics.Tex_Fog[Lists.Map[Editor_Maps.Selected].Fog.Texture]);
        int Speed = Lists.Map[Editor_Maps.Selected].Fog.Speed_Y;

        // Apenas se necessário
        if (FogY_Timer >= Environment.TickCount) return;
        if (Speed == 0) return;

        // Movimento para trás
        if (Speed < 0)
        {
            Globals.Fog_Y -= 1;
            if (Globals.Fog_Y < -Texture_Size.Height) Globals.Fog_Y = 0;
        }
        // Movimento para frente
        else
        {
            Globals.Fog_Y += 1;
            if (Globals.Fog_Y > Texture_Size.Height) Globals.Fog_Y = 0;
        }

        // Contagem
        if (Speed < 0) Speed *= -1;
        FogY_Timer = Environment.TickCount + 50 - Speed;
    }

    private static void Editor_Maps_Weather()
    {
        bool Stop = false, Move;
        byte First_Thunder = (byte)Audio.Sounds.Thunder_1;
        byte Last_Thunder = (byte)Audio.Sounds.Thunder_4;

        // Somente se necessário
        if (!Editor_Maps.Objects.Visible || Lists.Map[Editor_Maps.Selected].Weather.Type == 0 || !Editor_Maps.Objects.butVisualization.Checked)
        {
            if (Audio.Sound.List != null)
                if (Audio.Sound.List[(byte)Audio.Sounds.Rain].Status == SFML.Audio.SoundStatus.Playing) Audio.Sound.Stop_All();
            return;
        }

        // Clima do mapa
        Lists.Structures.Map_Weather Weather = Lists.Map[Editor_Maps.Selected].Weather;

        // Reproduz o som chuva
        if ((Globals.Weathers)Weather.Type == Globals.Weathers.Raining || (Globals.Weathers)Weather.Type == Globals.Weathers.Thundering)
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
        if (Globals.Lightning > 0)
            if (Thundering_Timer < Environment.TickCount)
            {
                Globals.Lightning -= 10;
                Thundering_Timer = Environment.TickCount + 25;
            }

        // Adiciona uma nova partícula
        for (int i = 1; i <= Lists.Weather.GetUpperBound(0); i++)
            if (!Lists.Weather[i].Visible)
            {
                if (Globals.GameRandom.Next(0, Globals.Max_Weather_Intensity - Weather.Intensity) == 0)
                {
                    if (!Stop)
                    {
                        // Cria a partícula
                        Lists.Weather[i].Visible = true;

                        // Cria a partícula de acordo com o seu tipo
                        switch ((Globals.Weathers)Weather.Type)
                        {
                            case Globals.Weathers.Thundering:
                            case Globals.Weathers.Raining: Weather_Rain_Create(i); break;
                            case Globals.Weathers.Snowing: Weather_Snow_Create(i); break;
                        }
                    }
                }

                Stop = true;
            }
            else
            {
                // Movimenta a partícula de acordo com o seu tipo
                switch ((Globals.Weathers)Weather.Type)
                {
                    case Globals.Weathers.Thundering:
                    case Globals.Weathers.Raining: Weather_Rain_Movement(i); break;
                    case Globals.Weathers.Snowing: Weather_Snow_Movement(i, Move); break;
                }

                // Reseta a partícula
                if (Lists.Weather[i].x > Editor_Maps.Map_Size.Width * Globals.Grid || Lists.Weather[i].y > Editor_Maps.Map_Size.Height * Globals.Grid)
                    Lists.Weather[i] = new Lists.Structures.Weather();
            }

        // Trovoadas
        if (Weather.Type == (byte)Globals.Weathers.Thundering)
            if (Globals.GameRandom.Next(0, Globals.Max_Weather_Intensity * 10 - Weather.Intensity * 2) == 0)
            {
                // Som do trovão
                int Thunder = Globals.GameRandom.Next(First_Thunder, Last_Thunder);
                Audio.Sound.Play((Audio.Sounds)Thunder);

                // Relâmpago
                if (Thunder < 6) Globals.Lightning = 190;
            }
    }

    private static void Weather_Rain_Create(int i)
    {
        // Define a velocidade e a posição da partícula
        Lists.Weather[i].Speed = Globals.GameRandom.Next(8, 13);

        if (Globals.GameRandom.Next(2) == 0)
        {
            Lists.Weather[i].x = -32;
            Lists.Weather[i].y = Globals.GameRandom.Next(-32, Editor_Maps.Objects.picMap.Height);
        }
        else
        {
            Lists.Weather[i].x = Globals.GameRandom.Next(-32, Editor_Maps.Objects.picMap.Width);
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
        Lists.Weather[i].Speed = Globals.GameRandom.Next(1, 3);
        Lists.Weather[i].y = -32;
        Lists.Weather[i].x = Globals.GameRandom.Next(-32, Editor_Maps.Objects.picMap.Width);
        Lists.Weather[i].Start = Lists.Weather[i].x;

        if (Globals.GameRandom.Next(2) == 0)
            Lists.Weather[i].Back = false;
        else
            Lists.Weather[i].Back = true;
    }

    private static void Weather_Snow_Movement(int i, bool Move = true)
    {
        int Difference = Globals.GameRandom.Next(0, Globals.Snow_Movement / 3);
        int x1 = Lists.Weather[i].Start + Globals.Snow_Movement + Difference;
        int x2 = Lists.Weather[i].Start - Globals.Snow_Movement - Difference;

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
        if (!Editor_Maps.Objects.Visible) goto stop;
        if (!Editor_Maps.Objects.butAudio.Checked) goto stop;
        if (!Editor_Maps.Objects.butVisualization.Checked) goto stop;
        if (Lists.Map[Editor_Maps.Selected].Music == 0) goto stop;

        // Inicia a música
        if (Audio.Music.Device == null || Audio.Music.Current != Lists.Map[Editor_Maps.Selected].Music)
            Audio.Music.Play((Audio.Musics)Lists.Map[Editor_Maps.Selected].Music);
        return;
    stop:
        // Para a música
        if (Audio.Music.Device != null) Audio.Music.Stop();
    }
}
