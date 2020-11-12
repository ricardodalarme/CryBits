using CryBits;
using CryBits.Editors.Forms;
using CryBits.Entities;

namespace Editors.Entities
{
    class TempMap
    {
        // Fumaças
        public static int Fog_X;
        public static int Fog_Y;

        // Clima
        public static byte Lightning;

        public static void UpdateWeather()
        {
            // Redimensiona a lista
            if (EditorMaps.Form != null)
                switch (EditorMaps.Form.Selected.Weather.Type)
                {
                    case Weathers.Thundering:
                    case Weathers.Raining: Lists.Weather = new MapWeatherParticle[Map.MaxRainParticles + 1]; break;
                    case Weathers.Snowing: Lists.Weather = new MapWeatherParticle[Map.MaxSnowParticles + 1]; break;
                }
        }
    }
}
