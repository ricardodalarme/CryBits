using CryBits.Client.Framework.Assets;
using CryBits.Client.Framework.Audio;
using CryBits.Definitions.Maps;
using CryBits.Editors.Forms.Maps;

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

    public void UpdateWeatherType()
    {
        Weather = [];
    }

    public void UpdateFog()
    {
        var win = EditorMapsWindow.Instance;
        if (win is not { IsOpen: true }) return;
        if (win.SelectedMap == null) return;
        if (win.SelectedMap.DefaultFog?.Texture == 0) return;
        UpdateFogX();
        UpdateFogY();
    }

    private void UpdateFogX()
    {
        var map = EditorMapsWindow.Instance!.SelectedMap!;
        var fog = map.DefaultFog;
        if (fog == null) return;
        var textureSize = Textures.Fogs[fog.Texture].ToSize();
        int speed = fog.SpeedX;

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
        var fog = map.DefaultFog;
        if (fog == null) return;
        var textureSize = Textures.Fogs[fog.Texture].ToSize();
        int speed = fog.SpeedY;

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

    public void UpdateWeather()
    {
        bool stop = false, move;

        var win = EditorMapsWindow.Instance;
        if (win?.SelectedMap == null) return;

        var weatherType = win.SelectedMap.DefaultWeather;
        if (!win.IsOpen || weatherType == WeatherType.None || !win.ShowVisualizationSafe)
        {
            if (AudioManager.Instance!.IsPlaying(Sounds.Rain))
                AudioManager.Instance.StopAllSounds();
            return;
        }

        if (weatherType is WeatherType.Rain or WeatherType.Thunder)
        {
            if (!AudioManager.Instance!.IsPlaying(Sounds.Rain))
                AudioManager.Instance.PlaySound(Sounds.Rain, true);
        }
        else if (AudioManager.Instance!.IsPlaying(Sounds.Rain))
        {
            AudioManager.Instance.StopAllSounds();
        }

        if (_snowTimer < Environment.TickCount64)
        {
            move = true;
            _snowTimer = Environment.TickCount64 + 35;
        }
        else
        {
            move = false;
        }

        if (Lightning > 0)
            if (_thunderingTimer < Environment.TickCount64)
            {
                Lightning -= 10;
                _thunderingTimer = Environment.TickCount64 + 25;
            }

        for (var i = 1; i <= Math.Max(0, Weather.GetUpperBound(0)); i++)
            if (!Weather[i].Visible)
            {
                if (Random.Shared.Next(0, 100) == 0)
                    if (!stop)
                    {
                        Weather[i].Visible = true;

                        switch (weatherType)
                        {
                            case WeatherType.Thunder:
                            case WeatherType.Rain: Weather[i].SetRain(); break;
                            case WeatherType.Snow: Weather[i].SetSnow(); break;
                        }
                    }

                stop = true;
            }
            else
            {
                switch (weatherType)
                {
                    case WeatherType.Thunder:
                    case WeatherType.Rain: Weather[i].MoveRain(); break;
                    case WeatherType.Snow: Weather[i].MoveSnow(move); break;
                }

                if (Weather[i].X > 16000 || Weather[i].Y > 16000)
                    Weather[i] = new MapWeatherParticleInstance();
            }

        if (weatherType == WeatherType.Thunder)
            if (Random.Shared.Next(0, 1000) == 0)
            {
                var thunderList = new[] { Sounds.Thunder1, Sounds.Thunder2, Sounds.Thunder3, Sounds.Thunder4 };
                var thunder = Random.Shared.Next(0, thunderList.Length);
                AudioManager.Instance?.PlaySound(thunderList[thunder]);

                if (thunder < 3) Lightning = 190;
            }
    }
}
