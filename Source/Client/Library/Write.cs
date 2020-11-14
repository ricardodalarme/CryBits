using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CryBits.Entities;
using static CryBits.Client.Logic.Game;

namespace CryBits.Client.Library
{
    internal static class Write
    {
        public static void Options()
        {
            // Escreve os dados
            using (var stream = new FileInfo(Directories.Options.FullName).OpenWrite())
                new BinaryFormatter().Serialize(stream, Option);
        }

        public static void Map(Map map)
        {
            // Escreve os dados
            using (var stream = new FileInfo(Directories.MapsData.FullName + map.ID + Directories.Format).OpenWrite())
                new BinaryFormatter().Serialize(stream, map);
        }
    }
}