using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Constants;
using CryBits.Entities.Map;
using CryBits.Enums;
using static CryBits.Globals;
using static CryBits.Utils;

namespace CryBits.Client.Framework.Entities.TempMap;

public class TempMapWeather
{
    public TempMapWeather(MapWeather data)
    {
        Data = data;
    }

    private MapWeather Data { get; }
    public byte Lightning { get; set; }
    public TempMapWeatherParticle[] Particles { get; private set; } = Array.Empty<TempMapWeatherParticle>();

    private int _snowTimer;
    private int _lightningTimer;

    public void Update()
    {
        bool stop = false, move;

        // Somente se necessário
        if (Data.Type == 0) return;

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
        for (short i = 1; i < Particles.Length; i++)
            if (!Particles[i].Visible)
            {
                if (MyRandom.Next(0, MaxWeatherIntensity - Data.Intensity) == 0)
                {
                    if (!stop)
                    {
                        // Cria a partícula
                        Particles[i].Visible = true;

                        // Cria a partícula de acordo com o seu tipo
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
                // Movimenta a partícula de acordo com o seu tipo
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

                // Reseta a partícula
                if (Particles[i].X > ScreenWidth || Particles[i].Y > ScreenHeight) Particles[i] = new TempMapWeatherParticle();
            }

        // Trovoadas
        if (Data.Type == Weather.Thundering)
            if (MyRandom.Next(0, MaxWeatherIntensity * 10 - Data.Intensity * 2) == 0)
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

    public void UpdateType()
    {
        // Para todos os sons
        Sound.StopAll();

        // Redimensiona a lista
        switch (Data.Type)
        {
            case Weather.Thundering:
            case Weather.Raining:
                // Reproduz o som chuva
                Sound.Play(Sounds.Rain, true);

                // Redimensiona a estrutura
                Particles = new TempMapWeatherParticle[MaxRainParticles + 1];
                break;
            case Weather.Snowing:
                Particles = new TempMapWeatherParticle[MaxSnowParticles + 1];
                break;
        }
    }
}