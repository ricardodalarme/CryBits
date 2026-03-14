using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Graphics;
using CryBits.Editors.Forms;
using CryBits.Entities.Map;
using System;
using static CryBits.Globals;
using static CryBits.Utils.RandomUtils;

namespace CryBits.Editors.Entities;

internal class MapInstance
{
    public static MapInstance Instance { get; } = new();

    private long _fogXTimer;
    private long _fogYTimer;
    private long _snowTimer;
    private long _thunderingTimer;

    public int FogX;
    public int FogY;

    public MapWeatherParticleInstance[] Weather = [];
    public byte Lightning;

    /// <summary>
    /// Resize the weather particle array for the current map weather type.
    /// </summary>
    public void UpdateWeatherType()
    {
        var win = EditorMapsWindow.Instance;
        if (win?.SelectedMap != null)
            Weather = win.SelectedMap.Weather.Type switch
            {
                Enums.Weather.Thundering or Enums.Weather.Raining =>
                    new MapWeatherParticleInstance[MaxRainParticles + 1],
                Enums.Weather.Snowing => new MapWeatherParticleInstance[MaxSnowParticles + 1],
                _ => Weather
            };
    }

    /// <summary>
    /// Update fog offsets and related timers for the selected map.
    /// </summary>
    public void UpdateFog()
    {
        var win = EditorMapsWindow.Instance;
        if (win == null || !win.IsOpen) return;
        if (win.SelectedMap == null) return;
        if (win.SelectedMap.Fog.Texture == 0) return;
        UpdateFogX();
        UpdateFogY();
    }

    private void UpdateFogX()
    {
        var map = EditorMapsWindow.Instance!.SelectedMap!;
        var textureSize = Textures.Fogs[map.Fog.Texture].ToSize();
        int speed = map.Fog.SpeedX;

        if (_fogXTimer >= Environment.TickCount64) return;
        if (speed == 0) return;

        if (speed < 0)
        {
            FogX--;
            if (FogX < -textureSize.Width) FogX = 0;
        }
        else
        {
            FogX++;
            if (FogX > textureSize.Width) FogX = 0;
        }

        if (speed < 0) speed *= -1;
        _fogXTimer = Environment.TickCount64 + 50 - speed;
    }

    private void UpdateFogY()
    {
        var map = EditorMapsWindow.Instance!.SelectedMap!;
        var textureSize = Textures.Fogs[map.Fog.Texture].ToSize();
        int speed = map.Fog.SpeedY;

        if (_fogYTimer >= Environment.TickCount64) return;
        if (speed == 0) return;

        if (speed < 0)
        {
            FogY--;
            if (FogY < -textureSize.Height) FogY = 0;
        }
        else
        {
            FogY++;
            if (FogY > textureSize.Height) FogY = 0;
        }

        if (speed < 0) speed *= -1;
        _fogYTimer = Environment.TickCount64 + 50 - speed;
    }

    /// <summary>
    /// Update weather particles, sounds and timers for the current map.
    /// </summary>
    public void UpdateWeather()
    {
        bool stop = false, move;

        var win = EditorMapsWindow.Instance;
        if (win?.SelectedMap == null) return;
        if (!win.IsOpen || win.SelectedMap.Weather.Type == 0 || !win.ShowVisualizationSafe)
        {
            if (AudioManager.Instance.IsPlaying(Sounds.Rain))
                AudioManager.Instance.StopAllSounds();
            return;
        }

        var weather = win.SelectedMap.Weather;

        if (weather.Type is Enums.Weather.Raining or Enums.Weather.Thundering)
        {
            if (!AudioManager.Instance.IsPlaying(Sounds.Rain))
                AudioManager.Instance.PlaySound(Sounds.Rain, true);
        }
        else if (AudioManager.Instance.IsPlaying(Sounds.Rain))
            AudioManager.Instance.StopAllSounds();

        if (_snowTimer < Environment.TickCount64)
        {
            move = true;
            _snowTimer = Environment.TickCount64 + 35;
        }
        else
            move = false;

        if (Lightning > 0)
            if (_thunderingTimer < Environment.TickCount64)
            {
                Lightning -= 10;
                _thunderingTimer = Environment.TickCount64 + 25;
            }

        for (var i = 1; i <= Weather.GetUpperBound(0); i++)
            if (!Weather[i].Visible)
            {
                if (MyRandom.Next(0, MaxWeatherIntensity - weather.Intensity) == 0)
                {
                    if (!stop)
                    {
                        Weather[i].Visible = true;

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
                switch (weather.Type)
                {
                    case Enums.Weather.Thundering:
                    case Enums.Weather.Raining: Weather[i].MoveRain(); break;
                    case Enums.Weather.Snowing: Weather[i].MoveSnow(move); break;
                }

                if (Weather[i].X > Map.Width * Grid || Weather[i].Y > Map.Height * Grid)
                    Weather[i] = new MapWeatherParticleInstance();
            }

        if (weather.Type == Enums.Weather.Thundering)
            if (MyRandom.Next(0, MaxWeatherIntensity * 10 - weather.Intensity * 2) == 0)
            {
                var thunderList = new[]
                {
                    Sounds.Thunder1,
                    Sounds.Thunder2,
                    Sounds.Thunder3,
                    Sounds.Thunder4
                };
                var thunder = MyRandom.Next(0, thunderList.Length);
                AudioManager.Instance.PlaySound(thunderList[thunder]);

                if (thunder < 3) Lightning = 190;
            }
    }
}
