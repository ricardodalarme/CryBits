using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Constants;
using CryBits.Entities.Map;
using CryBits.Enums;
using static CryBits.Globals;
using static CryBits.Utils;

namespace CryBits.Client.Framework.Entities.TempMap;

public class TempMapWeather(MapWeather data)
{
    private MapWeather Data { get; } = data;
    public byte Lightning { get; set; }
    public TempMapWeatherParticle[] Particles { get; private set; } = Array.Empty<TempMapWeatherParticle>();

    private int _snowTimer;
    private int _lightningTimer;

    public void Update()
    {
        bool stop = false, move;

        // Return early if no weather.
        if (Data.Type == 0) return;

        // Snow spawn timer check.
        if (_snowTimer < Environment.TickCount)
        {
            move = true;
            _snowTimer = Environment.TickCount + 35;
        }
        else
            move = false;

        // Lightning decay timer.
        if (Lightning > 0)
            if (_lightningTimer < Environment.TickCount)
            {
                Lightning -= 10;
                _lightningTimer = Environment.TickCount + 25;
            }

        // Spawn new particles when available.
        for (short i = 1; i < Particles.Length; i++)
            if (!Particles[i].Visible)
            {
                if (MyRandom.Next(0, MaxWeatherIntensity - Data.Intensity) == 0)
                {
                    if (!stop)
                    {
                        // Activate particle
                        Particles[i].Visible = true;

                        // Initialize particle according to weather type
                        switch (Data.Type)
                        {
                            case Weather.Thundering:
                            case Weather.Raining:
                                Particles[i].SetRain();
                                break;
                            case Weather.Snowing:
                                Particles[i].SetSnow();
                                break;
                        }
                    }
                }

                stop = true;
            }
            else
            {
                // Move particle based on weather type
                switch (Data.Type)
                {
                    case Weather.Thundering:
                    case Weather.Raining:
                        Particles[i].MoveRain();
                        break;
                    case Weather.Snowing:
                        Particles[i].MoveSnow(move);
                        break;
                }

                // Reset particle when out of bounds
                if (Particles[i].X > ScreenWidth || Particles[i].Y > ScreenHeight) Particles[i] = new TempMapWeatherParticle();
            }

        // Thunderstorm logic
        if (Data.Type == Weather.Thundering)
            if (MyRandom.Next(0, MaxWeatherIntensity * 10 - Data.Intensity * 2) == 0)
            {
                // Play thunder sound
                var thunderList = new[]
                {
                    Sounds.Thunder1,
                    Sounds.Thunder2,
                    Sounds.Thunder3,
                    Sounds.Thunder4
                };
                var thunder = MyRandom.Next(0, thunderList.Length);
                Sound.Play(thunderList[thunder]);

                // Flash lightning
                if (thunder < 3) Lightning = 190;
            }
    }

    public void UpdateType()
    {
        // Stop all sounds.
        Sound.StopAll();

        // Resize particle array for the current weather type.
        switch (Data.Type)
        {
            case Weather.Thundering:
            case Weather.Raining:
                // Play rain loop.
                Sound.Play(Sounds.Rain, true);

                // Allocate particle array for rain.
                Particles = new TempMapWeatherParticle[MaxRainParticles + 1];
                break;
            case Weather.Snowing:
                Particles = new TempMapWeatherParticle[MaxSnowParticles + 1];
                break;
        }
    }
}
