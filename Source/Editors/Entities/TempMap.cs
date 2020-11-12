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
            if (Editor_Maps.Form != null)
                switch (Editor_Maps.Form.Selected.Weather.Type)
                {
                    case Weathers.Thundering:
                    case Weathers.Raining: Lists.Weather = new MapWeatherParticle[Map.Max_Rain_Particles + 1]; break;
                    case Weathers.Snowing: Lists.Weather = new MapWeatherParticle[Map.Max_Snow_Particles + 1]; break;
                }
        }
    }
}
