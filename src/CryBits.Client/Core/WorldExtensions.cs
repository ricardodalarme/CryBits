using CryBits.Definitions.Maps;
using CryBits.Simulation.Core;

namespace CryBits.Client.Core;

internal static class WorldExtensions
{
    extension(World world)
    {
        public Map? CurrentMap
        {
            get => world.MapDefs.Values.FirstOrDefault();
            set
            {
                world.MapDefs.Clear();
                if (value != null)
                    world.MapDefs[value.Id] = value;
            }
        }
    }
}
