using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using static Logic.Utils;

namespace Library
{
    static class Write
    {
        public static void Options()
        {
            // Escreve os dados
            using (var Stream = new FileInfo(Directories.Options.FullName).OpenWrite())
                new BinaryFormatter().Serialize(Stream, Option);
        }

        public static void Map(Entities.Map Map)
        {
            // Escreve os dados
            using (var Stream = new FileInfo(Directories.Maps_Data.FullName + Map.ID.ToString() + Directories.Format).OpenWrite())
                new BinaryFormatter().Serialize(Stream, Map);
        }
    }
}