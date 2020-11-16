using CryBits.Editors.Forms;
using CryBits.Entities;

namespace CryBits.Editors.Entities
{
    internal static class TempMap
    {

        // Fumaças
        public static int FogX;
        public static int FogY;

        // Clima
        public static MapWeatherParticle[] Weather;
        public static byte Lightning;

        public static void UpdateWeather()
        {
            // Redimensiona a lista
            if (EditorMaps.Form != null)
                switch (EditorMaps.Form.Selected.Weather.Type)
                {
                    case Weathers.Thundering:
                    case Weathers.Raining: Weather = new MapWeatherParticle[Map.MaxRainParticles + 1]; break;
                    case Weathers.Snowing: Weather = new MapWeatherParticle[Map.MaxSnowParticles + 1]; break;
                }
        }
    }
}
