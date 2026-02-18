using System;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Entities.TempMap;
using CryBits.Client.Framework.Graphics;
using CryBits.Editors.Forms;
using CryBits.Entities.Map;
using SFML.Audio;
using static CryBits.Globals;
using static CryBits.Utils;
using Sound = CryBits.Client.Framework.Audio.Sound;

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
    public static TempMapWeatherParticle[] Weather = Array.Empty<TempMapWeatherParticle>();
    public static byte Lightning;

    public static void UpdateWeatherType()
    {
        // Redimensiona a lista
        if (EditorMaps.Form != null)
            switch (EditorMaps.Form.Selected.Weather.Type)
            {
                case Enums.Weather.Thundering:
                case Enums.Weather.Raining: Weather = new TempMapWeatherParticle[MaxRainParticles + 1]; break;
                case Enums.Weather.Snowing: Weather = new TempMapWeatherParticle[MaxSnowParticles + 1]; break;
            }
    }

    public static void UpdateFog()
    {
        // Faz a movimentação
        if (EditorMaps.Form?.Visible != true) return;
        if (EditorMaps.Form?.Selected == null) return;
        if (EditorMaps.Form?.Selected.Fog.Texture == 0) return;
        UpdateFogX();
        UpdateFogY();       
    }

    private static void UpdateFogX()
    {
        var textureSize = Textures.Fogs[EditorMaps.Form.Selected.Fog.Texture].ToSize();
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
        var textureSize = Textures.Fogs[EditorMaps.Form.Selected.Fog.Texture].ToSize();
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
        if (EditorMaps.Form?.Selected == null) return;
        if (EditorMaps.Form?.Visible != true || EditorMaps.Form.Selected.Weather.Type == 0 || !EditorMaps.Form.butVisualization.Checked)
        {
            if (Sound.List[Sounds.Rain].Status == SoundStatus.Playing) Sound.StopAll();
            return;
        }

        // Clima do mapa
        var weather = EditorMaps.Form.Selected.Weather;

        // Reproduz o som chuva
        if (weather.Type == Enums.Weather.Raining || weather.Type == Enums.Weather.Thundering)
        {
            if (Sound.List[Sounds.Rain].Status != SoundStatus.Playing)
                Sound.Play(Sounds.Rain);
        }
        else
        if (Sound.List[Sounds.Rain].Status == SoundStatus.Playing) Sound.StopAll();

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
        for (var i = 1; i <= Weather.GetUpperBound(0); i++)
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
                            case Enums.Weather.Raining: Weather[i].SetRain(); break;
                            case Enums.Weather.Snowing: Weather[i].SetSnow(); break;
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
                    case Enums.Weather.Raining: Weather[i].MoveRain(); break;
                    case Enums.Weather.Snowing: Weather[i].MoveSnow(move); break;
                }

                // Reseta a partícula
                if (Weather[i].X > Map.Width * Grid || Weather[i].Y > Map.Height * Grid)
                    Weather[i] = new TempMapWeatherParticle();
            }

        // Trovoadas
        if (weather.Type == Enums.Weather.Thundering)
            if (MyRandom.Next(0, MaxWeatherIntensity * 10 - weather.Intensity * 2) == 0)
            {
                // Som do trovão
                var thunderList = new[]
                {
                    Sounds.Thunder1,
                    Sounds.Thunder2,
                    Sounds.Thunder3,
                    Sounds.Thunder4
                };
                var thunder = MyRandom.Next(0, thunderList.Length);
                Sound.Play(thunderList[thunder]);

                // Relâmpago
                if (thunder < 3) Lightning = 190;
            }
    }
}
