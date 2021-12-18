using System;
using System.Drawing;
using CryBits.Editors.Forms;
using CryBits.Editors.Media.Graphics;
using CryBits.Entities;
using SFML.Audio;
using static CryBits.Globals;
using static CryBits.Utils;
using Sound = CryBits.Editors.Media.Audio.Sound;

namespace CryBits.Editors.Entities;

internal static class TempMap
{
    // Contadores
    private static int _fogXTimer;
    private static int _fogYTimer;
    private static int _snowTimer;
    private static int _thunderingTimer;

    // Fumaças
    public static int FogX;
    public static int FogY;

    // Clima
    public static MapWeatherParticle[] Weather = Array.Empty<MapWeatherParticle>();
    public static byte Lightning;

    public static void UpdateWeatherType()
    {
        // Redimensiona a lista
        if (EditorMaps.Form != null)
            switch (EditorMaps.Form.Selected.Weather.Type)
            {
                case Enums.Weather.Thundering:
                case Enums.Weather.Raining: Weather = new MapWeatherParticle[MaxRainParticles + 1]; break;
                case Enums.Weather.Snowing: Weather = new MapWeatherParticle[MaxSnowParticles + 1]; break;
            }
    }

    public static void UpdateFog()
    {
        // Faz a movimentação
        if (EditorMaps.Form?.Visible == true)
        {
            UpdateFogX();
            UpdateFogY();
        }
    }

    private static void UpdateFogX()
    {
        Size textureSize = Textures.Fogs[EditorMaps.Form.Selected.Fog.Texture].ToSize();
        int speed = EditorMaps.Form.Selected.Fog.SpeedX;

        // Apenas se necessário
        if (_fogXTimer >= Environment.TickCount) return;
        if (speed == 0) return;

        // Movimento para trás
        if (speed < 0)
        {
            FogX--;
            if (FogX < -textureSize.Width) FogX = 0;
        }
        // Movimento para frente
        else
        {
            FogX++;
            if (FogX > textureSize.Width) FogX = 0;
        }

        // Contagem
        if (speed < 0) speed *= -1;
        _fogXTimer = Environment.TickCount + 50 - speed;
    }

    private static void UpdateFogY()
    {
        Size textureSize = Textures.Fogs[EditorMaps.Form.Selected.Fog.Texture].ToSize();
        int speed = EditorMaps.Form.Selected.Fog.SpeedY;

        // Apenas se necessário
        if (_fogYTimer >= Environment.TickCount) return;
        if (speed == 0) return;

        // Movimento para trás
        if (speed < 0)
        {
            FogY--;
            if (FogY < -textureSize.Height) FogY = 0;
        }
        // Movimento para frente
        else
        {
            FogY++;
            if (FogY > textureSize.Height) FogY = 0;
        }

        // Contagem
        if (speed < 0) speed *= -1;
        _fogYTimer = Environment.TickCount + 50 - speed;
    }

    public static void UpdateWeather()
    {
        bool stop = false, move;

        // Somente se necessário
        if (EditorMaps.Form?.Visible != true || EditorMaps.Form.Selected.Weather.Type == 0 || !EditorMaps.Form.butVisualization.Checked)
        {
            if (Sound.List != null)
                if (Sound.List[(byte)Enums.Sound.Rain].Status == SoundStatus.Playing) Sound.StopAll();
            return;
        }

        // Clima do mapa
        MapWeather weather = EditorMaps.Form.Selected.Weather;

        // Reproduz o som chuva
        if (weather.Type == Enums.Weather.Raining || weather.Type == Enums.Weather.Thundering)
        {
            if (Sound.List[(byte)Enums.Sound.Rain].Status != SoundStatus.Playing)
                Sound.Play(Enums.Sound.Rain);
        }
        else
        if (Sound.List[(byte)Enums.Sound.Rain].Status == SoundStatus.Playing) Sound.StopAll();

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
            if (_thunderingTimer < Environment.TickCount)
            {
                Lightning -= 10;
                _thunderingTimer = Environment.TickCount + 25;
            }

        // Adiciona uma nova partícula
        for (int i = 1; i <= Weather.GetUpperBound(0); i++)
            if (!Weather[i].Visible)
            {
                if (MyRandom.Next(0, MaxWeatherIntensity - weather.Intensity) == 0)
                {
                    if (!stop)
                    {
                        // Cria a partícula
                        Weather[i].Visible = true;

                        // Cria a partícula de acordo com o seu tipo
                        switch (weather.Type)
                        {
                            case Enums.Weather.Thundering:
                            case Enums.Weather.Raining: SetRain(i); break;
                            case Enums.Weather.Snowing: SetSnow(i); break;
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
                    case Enums.Weather.Thundering:
                    case Enums.Weather.Raining: MoveRain(i); break;
                    case Enums.Weather.Snowing: SnowMove(i, move); break;
                }

                // Reseta a partícula
                if (Weather[i].X > Map.Width * Grid || Weather[i].Y > Map.Height * Grid)
                    Weather[i] = new MapWeatherParticle();
            }

        // Trovoadas
        if (weather.Type == Enums.Weather.Thundering)
            if (MyRandom.Next(0, (MaxWeatherIntensity * 10) - (weather.Intensity * 2)) == 0)
            {
                // Som do trovão
                int thunder = MyRandom.Next((byte)Enums.Sound.Thunder1, (byte)Enums.Sound.Thunder4);
                Sound.Play((Enums.Sound)thunder);

                // Relâmpago
                if (thunder < 6) Lightning = 190;
            }
    }

    private static void SetRain(int i)
    {
        // Define a velocidade e a posição da partícula
        Weather[i].Speed = MyRandom.Next(8, 13);

        if (MyRandom.Next(2) == 0)
        {
            Weather[i].X = -32;
            Weather[i].Y = MyRandom.Next(-32, EditorMaps.Form.picMap.Height);
        }
        else
        {
            Weather[i].X = MyRandom.Next(-32, EditorMaps.Form.picMap.Width);
            Weather[i].Y = -32;
        }
    }

    private static void MoveRain(int i)
    {
        // Movimenta a partícula
        Weather[i].X += Weather[i].Speed;
        Weather[i].Y += Weather[i].Speed;
    }

    private static void SetSnow(int i)
    {
        // Define a velocidade e a posição da partícula
        Weather[i].Speed = MyRandom.Next(1, 3);
        Weather[i].Y = -32;
        Weather[i].X = MyRandom.Next(-32, EditorMaps.Form.picMap.Width);
        Weather[i].Start = Weather[i].X;
        Weather[i].Back = MyRandom.Next(2) != 0;
    }

    private static void SnowMove(int i, bool move = true)
    {
        int difference = MyRandom.Next(0, SnowMovement / 3);
        int x1 = Weather[i].Start + SnowMovement + difference;
        int x2 = Weather[i].Start - SnowMovement - difference;

        // Faz com que a partícula volte
        if (x1 <= Weather[i].X)
            Weather[i].Back = true;
        else if (x2 >= Weather[i].X)
            Weather[i].Back = false;

        // Movimenta a partícula
        Weather[i].Y += Weather[i].Speed;

        if (move)
            if (Weather[i].Back)
                Weather[i].X--;
            else
                Weather[i].X++;
    }
}